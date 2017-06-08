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
using static Repository.Internal.Verify;

namespace Repository
{
    [Activity(Label = "Choose a Repository")]
    public class ChooseRepositoryActivity : Activity
    {
        private sealed class GitHubRepositoryAdapter : RecyclerView.Adapter
        {
            private readonly IReadOnlyList<Octokit.Repository> _repos;

            internal GitHubRepositoryAdapter(IReadOnlyList<Octokit.Repository> repos)
            {
                _repos = NotNull(repos);
            }

            public override int ItemCount => _repos.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var githubHolder = (GitHubRepositoryViewHolder)holder;
                githubHolder.RepoNameView.Text = _repos[position].Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.ChooseRepository_Layout, parent, attachToRoot: false);
                return new GitHubRepositoryViewHolder(view);
            }
        }

        private sealed class GitHubRepositoryViewHolder : RecyclerView.ViewHolder
        {
            public TextView RepoNameView { get; }

            internal GitHubRepositoryViewHolder(View view)
                : base(view)
            {
                RepoNameView = view.FindViewById<TextView>(Resource.Id.RepoNameView);
            }
        }

        private RecyclerView _repoView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChooseRepository);

            _repoView = FindViewById<RecyclerView>(Resource.Id.RepoView);
            _repoView.SetAdapter(await GetRepoViewAdapter());
            _repoView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private async Task<RecyclerView.Adapter> GetRepoViewAdapter()
        {
            var repos = await GitHub.Client.Repository.GetAllForCurrent();
            return new GitHubRepositoryAdapter(repos);
        }
    }
}