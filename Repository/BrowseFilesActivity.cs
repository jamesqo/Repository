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
    public class BrowseFilesActivity : Activity
    {
        private sealed class GitHubFileAdapter : RecyclerView.Adapter
        {
            private readonly long _repoId;

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

            public IReadOnlyList<Octokit.RepositoryContent> Contents { get; private set; }

            public event EventHandler<int> ItemClick;

            public override int ItemCount => Contents.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var githubHolder = (GitHubFileViewHolder)holder;
                var content = Contents[position];
                githubHolder.RepoNameView.Text = content.Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.BrowseFiles_CardView, parent, attachToRoot: false);
                return new GitHubFileViewHolder(view, OnClick);
            }

            private async Task RefreshContents()
            {
                Contents = await GitHub.Client.Repository.Content.GetAllContents(_repoId, _currentPath);
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);
        }

        private sealed class GitHubFileViewHolder : RecyclerView.ViewHolder
        {
            public TextView RepoNameView { get; }

            internal GitHubFileViewHolder(View view, Action<int> onClick)
                : base(view)
            {
                RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.FilenameView));

                view.Click += (sender, e) => onClick(AdapterPosition);
            }
        }

        private RecyclerView _fileView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BrowseFiles);

            _fileView = FindViewById<RecyclerView>(Resource.Id.FileView);
            _fileView.SetAdapter(await GetFileViewAdapter());
            _fileView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void Adapter_ItemClick(object sender, int e)
        {
            var adapter = (GitHubFileAdapter)sender;
            var content = adapter.Contents[e];
            
            switch (content.Type)
            {
                case Octokit.ContentType.Dir:
                case Octokit.ContentType.File:
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<RecyclerView.Adapter> GetFileViewAdapter()
        {
            long repoId = Intent.Extras.GetLong(Strings.BrowseFiles_RepoId);
            var adapter = await GitHubFileAdapter.Create(repoId);
            adapter.ItemClick += Adapter_ItemClick;
            return adapter;
        }
    }
}