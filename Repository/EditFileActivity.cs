using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Widget;
using Repository.Common.Validation;
using Repository.Editor.Android;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Highlighting;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.Internal.Threading;
using Repository.JavaInterop;
using _Highlighter = Repository.Editor.Highlighting.Highlighter;
using Path = System.IO.Path;

namespace Repository
{
    [Activity]
    public partial class EditFileActivity : Activity
    {
        // These properties are static instead of instance since they have must be preserved
        // when the activity is recreated, e.g. when the user changes the screen orientation.
        // However, they can't easily be serialized to a Bundle, so I just put them in static
        // fields.
        // (Although OriginalContent is a string, it can be arbitrarily long so it's not safe
        // to serialize it.)
        public static string OriginalContent { get; set; }
        private static TextColorer Colorer { get; set; }
        private static IHighlighter Highlighter { get; set; }
        private static CancellationTokenSource HighlightCanceller { get; set; }
        private static HighlightRequester HighlightRequester { get; set; }

        private EditText _editor;

        private string _path;

        public override void OnBackPressed()
        {
            // We don't need to continue highlighting this file's text if we're currently doing so.
            HighlightCanceller?.Cancel();

            OriginalContent = null;
            Colorer = null;
            Highlighter = null;
            HighlightCanceller = null;
            HighlightRequester = null;

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
            return _Highlighter.FromFileExtension(fileExtension)
                ?? _Highlighter.FromFirstLine(content.FirstLine())
                ?? _Highlighter.Plaintext;
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

            Verify.ValidState(HighlightRequester.IsHighlightRequested, "A highlight should have been requested.");

            HighlightCanceller = HighlightCanceller ?? new CancellationTokenSource();
            content = content ?? Colorer.Text.ToString();

            using (Colorer.Setup(FlushFrequency))
            {
                await Highlighter.Highlight(content, Colorer, HighlightCanceller.Token);
            }

            HighlightRequester.OnHighlightFinished();
            if (!HighlightRequester.IsHighlightRequested)
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
            ThreadingUtilities.PostToUIThread(async () => await HighlightContent());
        }

        private async Task SetupEditor(Bundle bundle)
        {
            var theme = GetEditorTheme();
            if (bundle == null)
            {
                // This is a fresh activity instance.
                Colorer = new TextColorer(OriginalContent, theme.Colors);
                Highlighter = GetHighlighter(filePath: _path, content: OriginalContent);

                HighlightRequester = new HighlightRequester(
                    onInitialRequest: OnHighlightRequested,
                    maxEditsBeforeRequest: 10);
                Colorer.Text.SetSpan(HighlightRequester);

                SetupEditorCore(theme, Colorer.Text);
                await HighlightContent(OriginalContent);
            }
            else
            {
                // This instance is being recreated from a previous instance of this activity.
                // Just use the state from that instance.
                SetupEditorCore(theme, Colorer.Text);
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