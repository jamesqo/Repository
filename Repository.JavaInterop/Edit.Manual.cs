using System;
using System.Diagnostics;
using Repository.Common;

namespace Repository.JavaInterop
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    public partial class Edit : IEquatable<Edit>
    {
        // Although the debugger is documented to display ToString() by default,
        // for some reason it isn't working for this type.
        private string DebuggerDisplay => ToString();

        public override bool Equals(object obj)
            => obj is Edit other && Equals(other);

        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString() => base.ToString();
    }
}