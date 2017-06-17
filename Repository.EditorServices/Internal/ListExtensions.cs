using System;
using System.Collections.Generic;
using System.Text;
using Repository.Common;

namespace Repository.EditorServices.Internal
{
    internal static class ListExtensions
    {
        public static ReadOnlyList<T> AsReadOnlyList<T>(this List<T> list) => ReadOnlyList<T>.Create(list);
    }
}
