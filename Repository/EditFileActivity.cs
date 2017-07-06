using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Repository.Editor.Highlighting;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.Internal.Editor.Highlighting;
using Repository.JavaInterop;
using static Repository.Common.Verify;
using Path = System.IO.Path;

namespace Repository
{
    [Activity]
    public class EditFileActivity : Activity
    {
        private EditText _editor;

        private string _content;
        private string _path;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _editor = FindViewById<EditText>(Resource.Id.Editor);
            }

            // TODO: Do this in other Activities, too?
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

            SetupFont();
            DisplayContent(ColorTheme.Default);
        }

        private void DisplayContent(IColorTheme theme)
        {
            _editor.SetBackgroundColor(theme.BackgroundColor);
            _editor.SetEditableFactory(NoCopyEditableFactory.Instance);

            var text = new ColoredText(_content);
            _editor.SetText(text, TextView.BufferType.Editable);
            Task.Factory.StartNew(HighlightContent, state: (text, theme));
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

            var (text, theme) = ((ColoredText, IColorTheme))state;
            using (var colorer = TextColorer.Create(text, theme))
            {
                GetHighlighter().Highlight(_content, colorer);
            }
        }

        private string ReadEditorContent() => EditorContent.Current;

        private void SetupFont() => _editor.Typeface = Typefaces.Inconsolata;
    }
}