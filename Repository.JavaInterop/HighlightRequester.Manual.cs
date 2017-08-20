using System;
using Repository.JavaInterop.Internal;

namespace Repository.JavaInterop
{
    public partial class HighlightRequester
    {
        public HighlightRequester(Action onInitialRequest, int maxEditsBeforeRequest)
            : this(new Runnable(onInitialRequest), maxEditsBeforeRequest)
        {
        }

        public bool IsHighlightRequested => _IsHighlightRequested();
    }
}