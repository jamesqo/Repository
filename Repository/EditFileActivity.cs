using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Repository.EditorServices.Highlighting;
using Repository.Internal;
using Repository.Internal.EditorServices.Highlighting;
using static Repository.Common.Verify;

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

            HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheParameters();

            ConfigureFont();
            DisplayContent(ColorTheme.Default);
        }

        private void ConfigureFont()
        {
            // TODO: Cache?
            var typeface = Typeface.CreateFromAsset(Assets, "fonts/Inconsolata.ttf");
            _editor.Typeface = typeface;
        }

        private void DisplayContent(IColorTheme theme)
        {
            _editor.SetBackgroundColor(theme.BackgroundColor);
            _editor.SetEditableFactory(NoCopyEditableFactory.Instance);

            var highlighter = GetHighlighter();
            using (var colorer = TextColorer.Create(_content, theme))
            {
                var coloredContent = highlighter.Highlight(_content, colorer);
                _editor.SetText(coloredContent, TextView.BufferType.Editable);
            }
        }

        private IHighlighter GetHighlighter()
        {
            var fileExtension = System.IO.Path.GetExtension(_path).TrimStart('.');
            var highlighter = Highlighter.FromFileExtension(fileExtension);

            if (highlighter != null)
            {
                return highlighter;
            }

            // TODO: What if there is no \n, or there's a very long first line?
            int firstLineEnd = _content.IndexOf('\n');
            var firstLine = _content.Substring(0, firstLineEnd);
            return Highlighter.FromFirstLine(firstLine) ?? Highlighter.Plaintext;
        }

        private void HideActionBar()
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        }

        private string ReadEditorContent() => EditorContent.Current;
    }
}