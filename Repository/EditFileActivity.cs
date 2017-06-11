using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Repository.Internal;
using Repository.Internal.SyntaxHighlighting;
using static Repository.Internal.Verify;

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

            void CacheExtras()
            {
                _content = NotNull(Intent.Extras.GetString(Strings.EditFile_Content));
                _path = NotNull(Intent.Extras.GetString(Strings.EditFile_Path));
            }

            base.OnCreate(savedInstanceState);

            HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheExtras();

            DisplayContent(SyntaxColorer.Default);
        }

        private void DisplayContent(SyntaxColorer colorer)
        {
            _editor.SetBackgroundColor(colorer.BackgroundColor);

            var highlighter = GetSyntaxHighlighter();
            var coloredContent = highlighter.Highlight(_content, colorer);
            _editor.SetText(coloredContent, TextView.BufferType.Editable);
        }

        private ISyntaxHighlighter GetSyntaxHighlighter()
        {
            var fileExtension = System.IO.Path.GetExtension(_path).TrimStart('.');
            var highlighter = SyntaxHighlighter.FromFileExtension(fileExtension);

            if (highlighter != null)
            {
                return highlighter;
            }

            int firstLineEnd = _content.IndexOf('\n');
            var firstLine = _content.Substring(0, firstLineEnd);
            return SyntaxHighlighter.FromFirstLine(firstLine) ?? SyntaxHighlighter.Plaintext;
        }

        private void HideActionBar()
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        }
    }
}