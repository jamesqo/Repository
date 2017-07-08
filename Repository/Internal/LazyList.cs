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
        private readonly List<T> _loaded;

        private LazyList(IEnumerable<T> source)
        {
            Verify.NotNull(source, nameof(source));

            _source = source;
            _enumerator = source.GetEnumerator();
            _loaded = new List<T>();
        }

        internal static LazyList<T> Create(IEnumerable<T> source) => new LazyList<T>(source);

        public int LoadedCount => _loaded.Count;

        public T this[int index] => _loaded[index];

        public T GetOrLoad(int index)
        {
            Verify.InRange(index >= 0 && LoadUntil(index), nameof(index));

            return _loaded[index];
        }

        private bool LoadUntil(int index)
        {
            Debug.Assert(index >= 0);

            int toLoad = index - (LoadedCount - 1);
            for (int i = 0; i < toLoad; i++)
            {
                if (!_enumerator.MoveNext())
                {
                    return false;
                }

                _loaded.Add(_enumerator.Current);
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}