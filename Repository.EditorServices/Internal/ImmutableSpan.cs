using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.Common;

namespace Repository.EditorServices.Internal
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct ImmutableSpan<T> : IEnumerable<T>
    {
        private ImmutableSpan(ImmutableArray<T> array, int index, int count)
        {
            Debug.Assert(!array.IsDefault);
            Debug.Assert(index >= 0 && count >= 0);
            Debug.Assert(array.Length - index >= count);

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

        public bool IsDefault => Array.IsDefault;

        public bool IsDefaultOrEmpty => IsDefault || IsEmpty;

        public bool IsEmpty => !IsDefault && Count == 0;

        public T this[int index] => Array[Index + index];

        private string DebuggerDisplay => $"[{Index}..{Index + Count})";

        public ImmutableSpan<T> Slice(int index) => Create(Array, Index + index, Count - index);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Array).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}