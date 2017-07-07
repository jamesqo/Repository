using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal static class MathUtilities
    {
        public static int Ceiling(int numerator, int denominator)
        {
            return ((numerator - 1) / denominator) + 1;
        }
    }
}