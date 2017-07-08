using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal static class MathUtilities
    {
        public static int Ceiling(int numerator, int denominator)
        {
            // Negative numbers or a numerator of 0 may not be invalid inputs, but there's no need to handle them yet.
            Debug.Assert(numerator > 0);
            Debug.Assert(denominator > 0);

            return ((numerator - 1) / denominator) + 1;
        }
    }
}