using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Repository.Editor.Highlighting;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.Internal.Editor.Highlighting;
using static Repository.Common.Verify;
using Debug = System.Diagnostics.Debug;
using Path = System.IO.Path;
using ThreadPriority = Android.OS.ThreadPriority;

namespace Repository
{
    [Activity]
    public partial class EditFileActivity : Activity
    {
        private RecyclerView _editor;

        private string _content;
        private string _path;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _editor = FindViewById<RecyclerView>(Resource.Id.Editor);
            }

            void CacheParameters()
            {
                _content = NotNull(ReadEditorContent());
                _path = NotNullOrEmpty(Intent.Extras.GetString(Strings.Extra_EditFile_Path));
            }

            base.OnCreate(savedInstanceState);

            this.HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheParameters();

            SetupEditor(EditorTheme.Default);
        }

        private IHighlighter GetHighlighter()
        {
            var fileExtension = Path.GetExtension(_path).TrimStart('.');
            return Highlighter.FromFileExtension(fileExtension)
                ?? Highlighter.FromFirstLine(_content.FirstLine())
                ?? Highlighter.Plaintext;
        }

        private async Task HighlightContent(TextColorer colorer)
        {
            using (colorer.Setup())
            {
                await GetHighlighter().Highlight(_content, colorer);
            }
        }

        private static string ReadEditorContent()
        {
            var content = EditorContent.Current;
            Debug.Assert(content != null); // We only read the content once.
            // No reason to keep the content alive after we're done using it.
            // The string can be arbitrarily large, so let the GC collect it ASAP.
            EditorContent.Current = null;
            return content;
        }

        private async Task SetupEditor(EditorTheme theme)
        {
            var colorer = TextColorer.Create(_content, theme.Colors);
            await HighlightContent(colorer);
        }
    }
}