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
    // TODO: "Select a File" instead?
    // TODO: Put the current directory path in tiny text below the activity label?
    [Activity(Label = "File View")]
    public class FileViewActivity : Activity
    {
        private sealed class GitHubFileAdapter : RecyclerView.Adapter
        {
            private readonly long _repoId;

            private IReadOnlyList<Octokit.RepositoryContent> _contents;
            private string _currentPath;

            private GitHubFileAdapter(long repoId)
            {
                _repoId = repoId;
                _currentPath = "/";
            }

            internal static async Task<GitHubFileAdapter> Create(long repoId)
            {
                var adapter = new GitHubFileAdapter(repoId);
                await adapter.RefreshContents();
                return adapter;
            }

            public override int ItemCount => _contents.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var githubHolder = (GitHubFileViewHolder)holder;
                var content = _contents[position];
                githubHolder.RepoNameView.Text = content.Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.FileView_CardView, parent, attachToRoot: false);
                return new GitHubFileViewHolder(view);
            }

            private async Task RefreshContents()
            {
                _contents = await GitHub.Client.Repository.Content.GetAllContents(_repoId, _currentPath);
            }
        }

        private sealed class GitHubFileViewHolder : RecyclerView.ViewHolder
        {
            public TextView RepoNameView { get; }

            internal GitHubFileViewHolder(View view)
                : base(view)
            {
                RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.FilenameView));
            }
        }

        private RecyclerView _fileView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FileView);

            long repoId = Intent.Extras.GetLong(Strings.FileView_RepoId);
            var adapter = await GitHubFileAdapter.Create(repoId);

            _fileView = FindViewById<RecyclerView>(Resource.Id.FileView);
            _fileView.SetAdapter(adapter);
            _fileView.SetLayoutManager(new LinearLayoutManager(this));
        }
    }
}