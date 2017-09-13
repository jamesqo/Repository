using System.Collections;
using System.Collections.Generic;

namespace Repository.JavaInterop
{
    public partial class EditQueue : IEnumerable<Edit>
    {
        public bool IsEmpty => _IsEmpty();

        public IEnumerator<Edit> GetEnumerator()
        {
            while (!IsEmpty)
            {
                yield return Remove();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}