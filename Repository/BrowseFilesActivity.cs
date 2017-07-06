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
using Octokit;
using Repository.Internal;
using Repository.Internal.Editor;
using static Repository.Common.Verify;
using Activity = Android.App.Activity;
using Debug = System.Diagnostics.Debug;

namespace Repository
{
    // TODO: Put the current directory path as the label.
    [Activity(Label = "File View")]
    public class BrowseFilesActivity : Activity
    {
        private class Adapter : RecyclerView.Adapter
        {
            private class ViewHolder : RecyclerView.ViewHolder
            {
                public TextView RepoNameView { get; }

                internal ViewHolder(View view, Action<int> onClick)
                    : base(view)
                {
                    RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.FilenameView));

                    view.Click += (sender, e) => onClick(AdapterPosition);
                }
            }

            private readonly long _repoId;
            private readonly Stack<string> _directoryStack;

            private IReadOnlyList<RepositoryContent> _contents;

            private Adapter(long repoId)
            {
                _repoId = repoId;
                _directoryStack = new Stack<string>();
            }

            internal static async Task<Adapter> Create(long repoId)
            {
                var adapter = new Adapter(repoId);
                await adapter.PushDirectory("/");
                return adapter;
            }

            public IReadOnlyList<RepositoryContent> Contents => _contents;

            public string CurrentDirectory => _directoryStack.Peek();

            public event EventHandler<int> ItemClick;

            public bool IsAtRoot => _directoryStack.Count == 1;

            public override int ItemCount => Contents.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var viewHolder = (ViewHolder)holder;
                var content = Contents[position];
                viewHolder.RepoNameView.Text = content.Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.BrowseFiles_CardView, parent, attachToRoot: false);
                return new ViewHolder(view, OnClick);
            }

            internal async Task<RepositoryContent> GetFullContent(string filePath)
            {
                var fullContents = await GitHub.Client.Repository.Content.GetAllContents(_repoId, filePath);
                return fullContents.Single();
            }

            internal Task PopDirectory()
            {
                Debug.Assert(!IsAtRoot);
                _directoryStack.Pop();
                return UpdateContents();
            }

            internal Task PushDirectory(string directory)
            {
                _directoryStack.Push(directory);
                return UpdateContents();
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);

            private IEnumerable<RepositoryContent> SortContents(IEnumerable<RepositoryContent> contents)
            {
                int GetPriority(ContentType type)
                {
                    switch (type)
                    {
                        case ContentType.Dir: return 0;
                        case ContentType.File: return 1;
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

        private RecyclerView _fileView;

        private long _repoId;

        public override async void OnBackPressed()
        {
            var adapter = (Adapter)_fileView.GetAdapter();
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

            void CacheParameters()
            {
                _repoId = Intent.Extras.GetLong(Strings.Extra_BrowseFiles_RepoId);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BrowseFiles);
            CacheViews();
            CacheParameters();

            await SetupFileView();
        }

        private void Adapter_ItemClick(object sender, int e)
        {
            var adapter = (Adapter)sender;
            var content = adapter.Contents[e];

            async void HandleDirectoryClick()
            {
                var subdir = adapter.CurrentDirectory + content.Name + "/";
                await adapter.PushDirectory(subdir);
            }

            async void HandleFileClick()
            {
                var filePath = adapter.CurrentDirectory + content.Name;
                var fullContent = await adapter.GetFullContent(filePath);

                StartEditFile(content: fullContent.Content, path: filePath);
            }

            switch (content.Type)
            {
                case ContentType.Dir:
                    HandleDirectoryClick();
                    break;
                case ContentType.File:
                    HandleFileClick();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<Adapter> GetFileViewAdapter()
        {
            var adapter = await Adapter.Create(_repoId);
            adapter.ItemClick += Adapter_ItemClick;
            return adapter;
        }

        private async Task SetupFileView()
        {
            _fileView.SetAdapter(await GetFileViewAdapter());
            _fileView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void StartEditFile(string content, string path)
        {
            // TODO: Other places where we don't pass in 'parameters' via the intent, but the other
            // activity still needs them, should accept those parameters in Start*?
            var intent = new Intent(this, typeof(EditFileActivity));
            WriteEditorContent(content);
            intent.PutExtra(Strings.Extra_EditFile_Path, path);
            StartActivity(intent);
        }

        private void WriteEditorContent(string content)
        {
            // The file content can be arbitrarily large, which makes it no good for Intent.PutExtra
            // and SharedPreferences. Just store it in a static field.
            EditorContent.Current = content;
        }
    }
}