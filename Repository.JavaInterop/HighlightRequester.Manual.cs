using System;
using Repository.JavaInterop.Internal;

namespace Repository.JavaInterop
{
    public partial class HighlightRequester
    {
        public HighlightRequester(Action<HighlightRequester> onInitialRequest, int maxEditsBeforeRequest)
            : this(new OnInitialRequestCallback(onInitialRequest), maxEditsBeforeRequest)
        {
        }

        public bool IsHighlightRequested => _IsHighlightRequested();
    }
}