using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Repository.Internal.Editor;
using Repository.Internal.Editor.Highlighting;
using static Repository.Common.Verify;

namespace Repository
{
    public partial class EditFileActivity
    {
        private class Adapter : RecyclerView.Adapter
        {
            private class ViewHolder : RecyclerView.ViewHolder
            {
                internal EditText EditorSegment { get; }

                internal ViewHolder(View view)
                    : base(view)
                {
                    EditorSegment = NotNull(view.FindViewById<EditText>(Resource.Id.EditorSegment));
                }
            }

            // Before the user touches the RecyclerView, OnBindViewHolder is called twice and 2 EditTexts are set up.
            internal const int InitialSegmentsRequested = 2;

            private readonly TextColorer _colorer;
            private readonly EditorTheme _theme;

            internal Adapter(TextColorer colorer, EditorTheme theme)
            {
                NotNull(colorer, nameof(colorer));
                NotNull(theme, nameof(theme));

                _colorer = colorer;
                _theme = theme;
            }

            public override int ItemCount => _colorer.SegmentCount;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var segment = ((ViewHolder)holder).EditorSegment;

                segment.InputType |= InputTypes.TextFlagNoSuggestions;
                segment.SetEditableFactory(NoCopyEditableFactory.Instance);
                segment.SetTypeface(_theme.Typeface, TypefaceStyle.Normal);
                segment.SetText(_colorer.GetSegment(position), TextView.BufferType.Editable);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.EditFile_EditorSegment, parent, attachToRoot: false);
                return new ViewHolder(view);
            }
        }
    }
}