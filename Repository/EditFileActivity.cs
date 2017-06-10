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
using static Repository.Internal.Verify;

namespace Repository
{
    [Activity]
    public class EditFileActivity : Activity
    {
        private EditText _editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _editor = FindViewById<EditText>(Resource.Id.Editor);
            }

            base.OnCreate(savedInstanceState);

            HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();

            DisplayContent();
        }

        private void DisplayContent()
        {
            var content = NotNull(Intent.Extras.GetString(Strings.EditFile_Content));
            var coloredContent = new SpannableString(content);
            var colorSpan = new ForegroundColorSpan(Color.Blue);
            coloredContent.SetSpan(colorSpan, 0, content.Length, SpanTypes.ExclusiveExclusive);
            _editor.SetText(coloredContent, TextView.BufferType.Editable);
        }

        private void HideActionBar()
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        }
    }
}