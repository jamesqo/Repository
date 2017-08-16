using System;
using Android.OS;
using Repository.JavaInterop.Internal;

namespace Repository.JavaInterop
{
    public partial class HighlightUpdateSource
    {
        public HighlightUpdateSource(Action doHighlightUpdate, Handler handler, int maxEditsBeforeUpdate)
            : this(new ActionRunnable(doHighlightUpdate), handler, maxEditsBeforeUpdate)
        {
        }
    }
}