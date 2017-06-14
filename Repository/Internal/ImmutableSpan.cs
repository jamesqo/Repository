using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct ImmutableSpan<T> : IEnumerable<T>
    {
        private ImmutableSpan(ImmutableArray<T> array, int index, int count)
        {
            // TODO: Argument validation
            Array = array;
            Index = index;
            Count = count;
        }

        public static implicit operator ImmutableSpan<T>(ImmutableArray<T> array)
        {
            return Create(array, 0, array.Length);
        }

        internal static ImmutableSpan<T> Create(ImmutableArray<T> array, int index, int count)
        {
            return new ImmutableSpan<T>(array, index, count);
        }

        public ImmutableArray<T> Array { get; }

        public int Count { get; }

        public int Index { get; }

        public T this[int index] => Array[Index + index];

        private string DebuggerDisplay => $"[{Index}..{Index + Count})";

        public ImmutableSpan<T> Slice(int index) => Create(Array, Index + index, Count - index);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Array).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}