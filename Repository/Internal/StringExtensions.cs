using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.Common;

namespace Repository.Internal
{
    internal static class StringExtensions
    {
        public static string FirstLine(this string text)
        {
            Verify.NotNull(text, nameof(text));

            int firstLineEnd = text.IndexOf('\n');
            return firstLineEnd == -1 ? text : text.Substring(0, firstLineEnd);
        }
    }
}