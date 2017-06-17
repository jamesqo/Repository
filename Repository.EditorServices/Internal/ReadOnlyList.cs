﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Repository.Common;

namespace Repository.EditorServices.Internal
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct ReadOnlyList<T>
    {
        public static ReadOnlyList<T> Empty { get; } = new List<T>().AsReadOnlyList();

        private readonly List<T> _list;

        private ReadOnlyList(List<T> list) => _list = list;

        internal static ReadOnlyList<T> Create(List<T> list) => new ReadOnlyList<T>(list);

        public int Count => _list.Count;

        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

        public bool IsDefault => _list == null;

        public T this[int index] => _list[index];

        private string DebuggerDisplay => $"Count = {Count}";
    }
}
