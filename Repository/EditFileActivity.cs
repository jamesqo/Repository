using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Widget;
using Repository.Common.Android.Threading;
using Repository.Common.Validation;
using Repository.Editor.Android;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Highlighting;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.JavaInterop;
using Path = System.IO.Path;

namespace Repository
{
    [Activity]
    public partial class EditFileActivity : Activity
    {
        public static string OriginalContent { get; set; }

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
            OriginalContent = null;
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
                _path = Intent.Extras.GetString(Strings.Extra_EditFile_Path).NotNullOrEmpty();
            }

            base.OnCreate(savedInstanceState);

            this.HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheParameters();

            try
            {
                await SetupEditor(savedInstanceState);
            }
            catch (TaskCanceledException)
            {
                // If highlighting is canceled because the user clicks the back button, just bail.
                return;
            }
        }

        private EditorTheme GetEditorTheme()
        {
            return EditorTheme.GetDefault(TypefaceProvider.GetInstance(Assets));
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
        /// <param name="content">The textual content of the editor.</param>
        private async Task HighlightContent(string content = null)
        {
            // This controls how often we flush work done by the colorer and yield
            // to pending work on the UI thread. A lower value means increased responsiveness
            // because we let other work, such as input/rendering code, run more often.
            const int FlushFrequency = 32;

            Verify.ValidState(_requester.IsHighlightRequested, "A highlight should have been requested.");

            _highlightCts = _highlightCts ?? new CancellationTokenSource();
            content = content ?? _colorer.Text.ToString();

            using (_colorer.Setup(FlushFrequency))
            {
                await _highlighter.Highlight(content, _colorer, _highlightCts.Token);
            }

            _requester.OnHighlightFinished();
            if (!_requester.IsHighlightRequested)
            {
                return;
            }

            await HighlightContent();
        }

        /// <summary>
        /// Handles an initial request for a highlight.
        /// </summary>
        private void OnHighlightRequested()
        {
            Verify.ValidState(ThreadingUtilities.IsRunningOnUIThread, "We should be running on the UI thread!");

            ThreadingUtilities.Post(async () => await HighlightContent());
        }

        private async Task SetupEditor(Bundle bundle)
        {
            var theme = GetEditorTheme();
            if (bundle == null)
            {
                // This is a fresh activity instance.
                _colorer = new TextColorer(OriginalContent, theme.Colors);
                _highlighter = GetHighlighter(filePath: _path, content: OriginalContent);

                _requester = new HighlightRequester(
                    onInitialRequest: OnHighlightRequested,
                    maxEditsBeforeRequest: 10);
                _colorer.Text.SetSpan(_requester);

                SetupEditorCore(theme, _colorer.Text);
                await HighlightContent(OriginalContent);
            }
            else
            {
                // This instance is being recreated from a previous instance of this activity.
                // All state from that instance has been transferred, except for one thing...
                // If we are in the middle of highlighting when the user rotates the device,
                // recreating the activity will cause the queued continuations that highlight the
                // source code to be lost. (The colorer implicitly queues these continuations when
                // it awaits Task.Yield().) We have to re-post those callbacks here.
                ThreadingUtilities.Post(DefaultYielder.MostRecentContinuation);
            }
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