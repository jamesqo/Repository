using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.JavaInterop
{
    public partial class ColoringList
    {
        public int Count => _Count();

        public Coloring this[int index]
        {
            get => Coloring.FromLong(Get(index));
            set => Set(index, value.ToLong());
        }
    }
}