using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Android.Text;
using Java.Lang;

namespace Repository.Internal.EditorServices.Highlighting
{
    // TODO: Should this be in an Internal namespace?
    internal class NoCopySpannableFactory : SpannableFactory
    {
        public override ISpannable NewSpannable(ICharSequence source) => (ISpannable)source;
    }
}