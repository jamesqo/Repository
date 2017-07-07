using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.JavaInterop
{
    public partial class ColoringList
    {
        public int Count => _Count();

        public long this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }
    }
}