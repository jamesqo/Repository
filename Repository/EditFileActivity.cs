using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Widget;
using Repository.Common;
using Repository.Editor.Highlighting;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.Internal.Editor.Highlighting;
using Repository.Internal.Threading;
using Repository.JavaInterop;
using Debug = System.Diagnostics.Debug;
using Path = System.IO.Path;

namespace Repository
{
    [Activity]
    public partial class EditFileActivity : Activity
    {
        private EditText _editor;

        private string _path;

        private TextColorer _colorer;
        private IHighlighter _highlighter;
        private CancellationTokenSource _highlightCts;
        private HighlightRequester _requester;

        public override void OnBackPressed()
        {
            // We don't need to continue highlighting this file's text if we're currently doing so.
            _highlightCts?.Cancel();
            base.OnBackPressed();
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _editor = FindViewById<EditText>(Resource.Id.EditFile_Editor);
            }

            void CacheParameters()
            {
                // The content is not cached since it may be an arbitrarily large string.
                // If we stored it in a field, we would want to clear that field ASAP anyway
                // to allow the GC to collect the string.
                _path = Intent.Extras.GetString(Strings.Extra_EditFile_Path).NotNullOrEmpty();
            }

            base.OnCreate(savedInstanceState);

            this.HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheParameters();

            if (await SetupEditor(EditorTheme.Default).BecomesCanceled())
            {
                return;
            }
        }

        /// <summary>
        /// Gets the best syntax highlighter given a file's path and contents.
        /// </summary>
        private static IHighlighter GetHighlighter(string filePath, string content)
        {
            var fileExtension = Path.GetExtension(filePath).TrimStart('.');
            return Highlighter.FromFileExtension(fileExtension)
                ?? Highlighter.FromFirstLine(content.FirstLine())
                ?? Highlighter.Plaintext;
        }

        /// <summary>
        /// Highlights the content of the editor.
        /// </summary>
        private Task HighlightContent()
        {
            string newContent = _colorer.Text.ToString();
            return HighlightContent(newContent);
        }

        /// <summary>
        /// Highlights the content of the editor.
        /// </summary>
        /// <param name="content">The textual content of the editor.</param>
        private async Task HighlightContent(string content)
        {
            Debug.Assert(_requester.IsHighlightRequested);

            await HighlightContentCore(content);

            _requester.OnHighlightFinished();
            if (!_requester.IsHighlightRequested)
            {
                return;
            }

            await HighlightContent();
        }

        /// <summary>
        /// Highlights the content of the editor.
        /// </summary>
        /// <param name="content">The textual content of the editor.</param>
        private async Task HighlightContentCore(string content)
        {
            // This controls how often we flush work done by the colorer and yield
            // to pending work on the UI thread. A lower value means increased responsiveness
            // because we let other work, such as input/rendering code, run more often.
            const int FlushFrequency = 32;

            _highlightCts = _highlightCts ?? new CancellationTokenSource();

            using (_colorer.Setup(FlushFrequency))
            {
                // Do not reference `content` past this line.
                // Doing so will cause it to be stored in a field in the async state
                // machine for this method. Storing a reference to it will prevent the GC
                // from collecting the string while the highlighter is running, which is
                // undesirable for large files.
                await _highlighter.Highlight(content, _colorer, _highlightCts.Token);
            }
        }

        /// <summary>
        /// Handles an initial request for a highlight.
        /// </summary>
        private void OnHighlightRequested()
        {
            ThreadingUtilities.PostToUIThread(async () => await HighlightContent());
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

        private Task SetupEditor(EditorTheme theme)
        {
            var content = ReadEditorContent();
            _colorer = new TextColorer(content, theme.Colors);
            _highlighter = GetHighlighter(filePath: _path, content: content);

            _requester = new HighlightRequester(
                onInitialRequest: OnHighlightRequested,
                maxEditsBeforeRequest: 10);
            _colorer.Text.SetSpan(_requester);

            SetupEditorCore(theme, _colorer.Text);
            return HighlightContent(content);
        }

        private void SetupEditorCore(EditorTheme theme, EditorText text)
        {
            _editor.InputType |= InputTypes.TextFlagNoSuggestions;
            _editor.SetEditableFactory(NoCopyEditableFactory.Instance);
            _editor.SetTypeface(theme.Typeface, TypefaceStyle.Normal);
            _editor.SetText(text, TextView.BufferType.Editable);
        }
    }
}