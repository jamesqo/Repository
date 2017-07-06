using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Repository.Common;
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
                    // TODO: EditorSegment = NotNull(view.FindViewById<EditText>(Resource.Id.EditorSegment));
                }
            }

            private readonly TextColorer _colorer;
            private readonly EditorTheme _theme;

            internal Adapter(string content, EditorTheme theme)
            {
                NotNull(content, nameof(content));
                NotNull(theme, nameof(theme));

                _colorer = TextColorer.Create(content, theme.Colors);
                _theme = theme;
            }

            public override int ItemCount => _colorer.SegmentCount;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var segment = ((ViewHolder)holder).EditorSegment;

                segment.SetEditableFactory(NoCopyEditableFactory.Instance);
                segment.SetTypeface(_theme.Typeface, TypefaceStyle.Normal);
                segment.SetText(_colorer.GetSegment(position), TextView.BufferType.Editable);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(/* TODO: Resource.Layout.EditFile_EditorSegment */ 0, parent, attachToRoot: false);
                return new ViewHolder(view);
            }
        }
    }
}