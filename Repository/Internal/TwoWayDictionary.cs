using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal class TwoWayDictionary<T1, T2>
    {
        private readonly Dictionary<T1, T2> _forwards;
        private readonly Dictionary<T2, T1> _backwards;

        public TwoWayDictionary(
            IEqualityComparer<T1> forwardsComparer = null,
            IEqualityComparer<T2> backwardsComparer = null)
        {
            _forwards = new Dictionary<T1, T2>(forwardsComparer);
            _backwards = new Dictionary<T2, T1>(backwardsComparer);
        }

        public int Count
        {
            get
            {
                Debug.Assert(_forwards.Count == _backwards.Count);
                return _forwards.Count;
            }
        }

        public void Add(T1 first, T2 second)
        {
            _forwards.Add(first, second);
            _backwards.Add(second, first);
        }

        public T1 GetBackwards(T2 key) => _backwards[key];

        public bool TryGetForwards(T1 key, out T2 value) => _forwards.TryGetValue(key, out value);
    }
}