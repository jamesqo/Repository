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

        public int Capacity => _array?.Length ?? 0;

        public int Count => _count;

        public T this[int index]
        {
            get
            {
                Debug.Assert(index < _count);
                return _array[index];
            }
        }

        private string DebuggerDisplay => $"{nameof(Count)}: {Count}";

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_array, 0, array, arrayIndex, _count);
        }

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