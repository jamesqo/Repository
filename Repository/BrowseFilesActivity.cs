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
using static System.Diagnostics.Debug;

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
            private readonly Stack<string> _directoryStack;

            private IReadOnlyList<Octokit.RepositoryContent> _contents;

            private GitHubFileAdapter(long repoId)
            {
                _repoId = repoId;
                _directoryStack = new Stack<string>();
            }

            internal static async Task<GitHubFileAdapter> Create(long repoId)
            {
                var adapter = new GitHubFileAdapter(repoId);
                await adapter.PushDirectory("/");
                return adapter;
            }

            public IReadOnlyList<Octokit.RepositoryContent> Contents => _contents;

            public string CurrentDirectory => _directoryStack.Peek();

            public event EventHandler<int> ItemClick;

            public bool IsAtRoot => _directoryStack.Count == 1;

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

            internal Task PopDirectory()
            {
                Assert(!IsAtRoot);
                _directoryStack.Pop();
                return UpdateContents();
            }

            internal Task PushDirectory(string directory)
            {
                _directoryStack.Push(directory);
                return UpdateContents();
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);

            private IEnumerable<Octokit.RepositoryContent> SortContents(IEnumerable<Octokit.RepositoryContent> contents)
            {
                int GetPriority(Octokit.ContentType type)
                {
                    switch (type)
                    {
                        case Octokit.ContentType.Dir: return 0;
                        case Octokit.ContentType.File: return 1;
                        default: throw new NotImplementedException();
                    }
                }

                return contents.OrderBy(c => GetPriority(c.Type)).ThenBy(c => c.Name);
            }

            private async Task UpdateContents()
            {
                var unsortedContents = await GitHub.Client.Repository.Content.GetAllContents(_repoId, CurrentDirectory);
                _contents = SortContents(unsortedContents).ToReadOnly();
                NotifyDataSetChanged();
            }
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

        public override async void OnBackPressed()
        {
            var adapter = (GitHubFileAdapter)_fileView.GetAdapter();
            if (adapter.IsAtRoot)
            {
                base.OnBackPressed();
            }
            else
            {
                await adapter.PopDirectory();
            }
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _fileView = FindViewById<RecyclerView>(Resource.Id.FileView);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BrowseFiles);
            CacheViews();

            _fileView.SetAdapter(await GetFileViewAdapter());
            _fileView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private async void Adapter_ItemClick(object sender, int e)
        {
            var adapter = (GitHubFileAdapter)sender;
            var content = adapter.Contents[e];
            
            switch (content.Type)
            {
                case Octokit.ContentType.Dir:
                    var subdir = adapter.CurrentDirectory + content.Name + "/";
                    await adapter.PushDirectory(subdir);
                    break;
                case Octokit.ContentType.File:
                    var intent = new Intent(this, typeof(EditFileActivity));
                    intent.PutExtra(Strings.EditFile_Content, content.Content);
                    StartActivity(intent);
                    break;
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