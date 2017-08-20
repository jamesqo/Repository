using System;
using System.Collections.Generic;
using System.Text;
using Repository.Common;

namespace Repository.Editor.Internal
{
    internal static class ListExtensions
    {
        public static ReadOnlyList<T> AsReadOnlyList<T>(this List<T> list) => new ReadOnlyList<T>(list);
    }
}
