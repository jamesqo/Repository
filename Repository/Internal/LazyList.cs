using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.Common;

namespace Repository.Internal
{
    internal class LazyList<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly IEnumerator<T> _enumerator;
        private readonly List<T> _list;

        private LazyList(IEnumerable<T> source)
        {
            Verify.NotNull(source, nameof(source));

            _source = source;
            _enumerator = source.GetEnumerator();
            _list = new List<T>();
        }

        internal static LazyList<T> Create(IEnumerable<T> source) => new LazyList<T>(source);

        public T ElementAt(int index)
        {
            Verify.InRange(index >= 0 && ReadUntil(index), nameof(index));

            return _list[index];
        }

        private bool ReadUntil(int index)
        {
            Debug.Assert(index >= 0);

            int toRead = index - (_list.Count - 1);
            for (int i = 0; i < toRead; i++)
            {
                if (!_enumerator.MoveNext())
                {
                    return false;
                }

                _list.Add(_enumerator.Current);
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}