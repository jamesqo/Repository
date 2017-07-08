﻿using System.Threading;
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

        private void HighlightContent(object state)
        {
            Process.SetThreadPriority(ThreadPriority.Background);

            var (colorer, barrier) = ((TextColorer, Barrier))state;

            // Signal the UI thread once we've highlighted all the segments it will initially request.
            // If this is not done, watchers will be attached from the UI thread during EditText.SetText().
            // When this bg thread calls SetSpan() to highlight the EditText's ColoredText, these watchers
            // will be invoked and attempt to modify the UI from this thread.
            // The fix is to ensure we finish highlighting a segment before any watchers are attached.
            int index = colorer.GetSegmentEnd(Adapter.InitialSegmentsRequested - 1);
            colorer.WhenIndexPassed(index, barrier.SignalAndWait);

            using (colorer.Setup())
            {
                GetHighlighter().Highlight(_content, colorer);
            }
        }

        private string ReadEditorContent() => EditorContent.Current;

        private void SetupEditor(EditorTheme theme)
        {
            var colorer = TextColorer.Create(_content, theme.Colors);
            var barrier = new Barrier(2);
            Task.Factory.StartNew(HighlightContent, state: (colorer, barrier));
            barrier.SignalAndWait();

            _editor.SetAdapter(new Adapter(colorer, theme));
            _editor.SetLayoutManager(new LinearLayoutManager(this));
        }
    }
}