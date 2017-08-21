using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Repository.Internal.Collections
{
    internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        private ReferenceEqualityComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object x, object y) => x == y;

        int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}