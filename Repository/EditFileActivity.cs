﻿using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Widget;
using Repository.Editor.Highlighting;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.Internal.Editor.Highlighting;
using Repository.Internal.Threading;
using Repository.JavaInterop;
using static Repository.Common.Verify;
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
                _path = NotNullOrEmpty(Intent.Extras.GetString(Strings.Extra_EditFile_Path));
            }

            base.OnCreate(savedInstanceState);

            this.HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheParameters();

            try
            {
                await SetupEditor(EditorTheme.Default);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }

        private static IHighlighter GetHighlighter(string filePath, string content)
        {
            var fileExtension = Path.GetExtension(filePath).TrimStart('.');
            return Highlighter.FromFileExtension(fileExtension)
                ?? Highlighter.FromFirstLine(content.FirstLine())
                ?? Highlighter.Plaintext;
        }

        private Task HighlightContent(HighlightRequester requester)
        {
            var editorText = _colorer.Text;
            editorText.ClearColorSpans();
            string newContent = editorText.ToString();
            return HighlightContent(newContent, requester);
        }

        private async Task HighlightContent(string content, HighlightRequester requester)
        {
            // This controls how often we flush work done by the colorer and yield
            // to pending work on the UI thread. A lower value means increased responsiveness
            // because we let other work, such as input/rendering code, run more often.
            const int FlushFrequency = 32;

            _highlightCts = _highlightCts ?? new CancellationTokenSource();

            Debug.Assert(requester.IsHighlightRequested);
            using (_colorer.Setup(FlushFrequency))
            {
                // Do not reference `content` past this line.
                // Doing so will cause it to be stored in a field in the async state
                // machine for this method. Storing a reference to it will prevent the GC
                // from collecting the string while the highlighter is running, which is
                // undesirable for large files.
                await _highlighter.Highlight(content, _colorer, _highlightCts.Token);
            }

            requester.OnHighlightFinished();
            if (!requester.IsHighlightRequested)
            {
                return;
            }

            await HighlightContent(requester);
        }

        private void OnHighlightRequested(HighlightRequester requester)
            => ThreadingUtilities.PostToUIThread(() => HighlightContent(requester));

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
            _colorer = TextColorer.Create(content, theme.Colors);
            _highlighter = GetHighlighter(filePath: _path, content: content);

            // TODO: Ensure DoHighlightUpdate doesn't get called until the previous one finishes?
            var requester = new HighlightRequester(
                OnHighlightRequested,
                10);
            _colorer.Text.SetSpan(requester);

            _editor.InputType |= InputTypes.TextFlagNoSuggestions;
            _editor.SetEditableFactory(NoCopyEditableFactory.Instance);
            _editor.SetTypeface(theme.Typeface, TypefaceStyle.Normal);
            _editor.SetText(_colorer.Text, TextView.BufferType.Editable);

            return HighlightContent(content, requester);
        }
    }
}