using System;
using Android.OS;

namespace Repository.JavaInterop
{
    public partial class HighlightUpdateTrigger
    {
        public HighlightUpdateTrigger(Action doHighlightUpdate, Handler handler, int maxEditsBeforeTrigger)
            : this(new ActionRunnable(doHighlightUpdate), handler, maxEditsBeforeTrigger)
        {
        }
    }
}