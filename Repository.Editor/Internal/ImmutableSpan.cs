using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.Common;
using Repository.Common.Validation;

namespace Repository.Editor.Internal
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct ImmutableSpan<T> : IEnumerable<T>
    {
        internal ImmutableSpan(ImmutableArray<T> array, int index, int count)
        {
            Verify.Argument(!array.IsDefault, nameof(array));
            Verify.InRange(index >= 0, nameof(index));
            Verify.InRange(count >= 0 && array.Length - index >= count, nameof(count));

            Array = array;
            Index = index;
            Count = count;
        }

        public static implicit operator ImmutableSpan<T>(ImmutableArray<T> array)
        {
            return new ImmutableSpan<T>(array, 0, array.Length);
        }

        public ImmutableArray<T> Array { get; }

        public int Count { get; }

        public int Index { get; }

        public bool IsDefault => Array.IsDefault;

        public bool IsDefaultOrEmpty => IsDefault || IsEmpty;

        public bool IsEmpty => !IsDefault && Count == 0;

        public T this[int index] => Array[Index + index];

        private string DebuggerDisplay => $"[{Index}..{Index + Count})";

        public ImmutableSpan<T> Slice(int index)
            => new ImmutableSpan<T>(Array, Index + index, Count - index);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Array).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}