using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Repository.Internal;
using static Repository.Common.Verify;

namespace Repository
{
    [Activity(Label = "Choose a Repository")]
    public class ChooseRepoActivity : Activity
    {
        private class GitHubRepoAdapter : RecyclerView.Adapter
        {
            private class ViewHolder : RecyclerView.ViewHolder
            {
                public TextView RepoNameView { get; }

                internal ViewHolder(View view, Action<int> onClick)
                    : base(view)
                {
                    RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.RepoNameView));

                    view.Click += (sender, e) => onClick(AdapterPosition);
                }
            }

            internal GitHubRepoAdapter(IReadOnlyList<Octokit.Repository> repos)
            {
                // TODO: What if there are no repos?
                Repos = NotNull(repos, nameof(repos));
            }

            public event EventHandler<int> ItemClick;

            public override int ItemCount => Repos.Count;

            public IReadOnlyList<Octokit.Repository> Repos { get; }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var viewHolder = (ViewHolder)holder;
                viewHolder.RepoNameView.Text = Repos[position].Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.ChooseRepo_CardView, parent, attachToRoot: false);
                return new ViewHolder(view, OnClick);
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);
        }

        private RecyclerView _repoView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _repoView = FindViewById<RecyclerView>(Resource.Id.RepoView);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChooseRepo);
            CacheViews();

            _repoView.SetAdapter(await GetRepoViewAdapter());
            _repoView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void Adapter_ItemClick(object sender, int e)
        {
            var adapter = (GitHubRepoAdapter)sender;
            var repo = adapter.Repos[e];
            StartBrowseFiles(repoId: repo.Id);
        }

        private async Task<RecyclerView.Adapter> GetRepoViewAdapter()
        {
            var repos = await GitHub.Client.Repository.GetAllForCurrent();
            var adapter = new GitHubRepoAdapter(repos);
            adapter.ItemClick += Adapter_ItemClick;
            return adapter;
        }

        private void StartBrowseFiles(long repoId)
        {
            var intent = new Intent(this, typeof(BrowseFilesActivity));
            intent.PutExtra(Strings.Extra_BrowseFiles_RepoId, repoId);
            StartActivity(intent);
        }
    }
}