using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.Common;

namespace Repository.Internal
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct ArrayBuilder<T>
    {
        private T[] _array;
        private int _count;

        public void Add(T item)
        {
            if (_count == Capacity)
            {
                Resize();
            }

            _array[_count++] = item;
        }

        public T[] Array => _array;

        public int Capacity => _array?.Length ?? 0;

        public int Count => _count;

        private string DebuggerDisplay => $"{nameof(Count)}: {Count}";

        private void Resize()
        {
            Debug.Assert(_count == Capacity);

            if (_count == 0)
            {
                _array = new T[4];
                return;
            }

            int newCapacity = 2 * _count;
            var newArray = new T[newCapacity];
            Array.Copy(_array, 0, newArray, 0, _count);
            _array = newArray;
        }
    }
}