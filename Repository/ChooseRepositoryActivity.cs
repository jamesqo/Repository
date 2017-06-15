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
    public class ChooseRepositoryActivity : Activity
    {
        private sealed class GitHubRepositoryAdapter : RecyclerView.Adapter
        {
            internal GitHubRepositoryAdapter(IReadOnlyList<Octokit.Repository> repos)
            {
                // TODO: What if there are no repos?
                Repos = NotNull(repos);
            }

            public event EventHandler<int> ItemClick;

            public override int ItemCount => Repos.Count;

            public IReadOnlyList<Octokit.Repository> Repos { get; }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var githubHolder = (GitHubRepositoryViewHolder)holder;
                githubHolder.RepoNameView.Text = Repos[position].Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.ChooseRepository_CardView, parent, attachToRoot: false);
                return new GitHubRepositoryViewHolder(view, OnClick);
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);
        }

        private sealed class GitHubRepositoryViewHolder : RecyclerView.ViewHolder
        {
            public TextView RepoNameView { get; }

            internal GitHubRepositoryViewHolder(View view, Action<int> onClick)
                : base(view)
            {
                RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.RepoNameView));

                view.Click += (sender, e) => onClick(AdapterPosition);
            }
        }

        private RecyclerView _repoView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _repoView = FindViewById<RecyclerView>(Resource.Id.RepoView);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChooseRepository);
            CacheViews();

            _repoView.SetAdapter(await GetRepoViewAdapter());
            _repoView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void Adapter_ItemClick(object sender, int e)
        {
            var adapter = (GitHubRepositoryAdapter)sender;
            var repo = adapter.Repos[e];
            StartBrowseFiles(repoId: repo.Id);
        }

        private async Task<RecyclerView.Adapter> GetRepoViewAdapter()
        {
            var repos = await GitHub.Client.Repository.GetAllForCurrent();
            var adapter = new GitHubRepositoryAdapter(repos);
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