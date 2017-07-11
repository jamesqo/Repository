using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace Repository
{
    public partial class EditFileActivity
    {
        private class LayoutManager : LinearLayoutManager
        {
            internal LayoutManager(Context context)
                : base(context)
            {
            }

            public override bool RequestChildRectangleOnScreen(RecyclerView parent, View child, Rect rect, bool immediate, bool focusedChildVisible)
            {
                return false;
            }
        }
    }
}