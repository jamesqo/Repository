using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Octokit;
using Repository.Internal;
using Repository.Internal.Editor;
using Activity = Android.App.Activity;

namespace Repository
{
    [Activity(Label = "File View")]
    public partial class BrowseFilesActivity : Activity
    {
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
                _fileView = FindViewById<RecyclerView>(Resource.Id.BrowseFiles_FileView);
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

        private async Task<Adapter> GetAdapter()
        {
            var adapter = await Adapter.Create(_repoId);
            adapter.ItemClick += Adapter_ItemClick;
            return adapter;
        }

        private async Task SetupFileView()
        {
            _fileView.SetAdapter(await GetAdapter());
            _fileView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void StartEditFile(string content, string path)
        {
            var intent = new Intent(this, typeof(EditFileActivity));
            WriteEditorContent(content);
            intent.PutExtra(Strings.Extra_EditFile_Path, path);
            StartActivity(intent);
        }

        private static void WriteEditorContent(string content)
        {
            // The file content can be arbitrarily large, which makes it no good for Intent.PutExtra
            // and SharedPreferences. Just store it in a static field.
            EditorContent.Current = content;
        }
    }
}