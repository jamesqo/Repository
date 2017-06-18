using System;
using System.Collections;
using System.Collections.Generic;

namespace Repository.JavaInterop
{
    // For some reason, the Xamarin bindings library doesn't take care of the IEnumerable
    // implementation if the Java class implements CharSequence. This is a workaround for that.
    public partial class ColoredText
    {
        IEnumerator<char> IEnumerable<char>.GetEnumerator() => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }
}
