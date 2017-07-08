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

        public static int IndexOfNth(this string text, char value, int n, int startIndex = 0)
        {
            Verify.NotNull(text, nameof(text));
            Verify.InRange(n > 0, nameof(n));
            Verify.InRange(startIndex >= 0 && startIndex <= text.Length, nameof(startIndex));

            int occurrences = 0;
            for (int i = startIndex; i < text.Length; i++)
            {
                if (text[i] == value && ++occurrences == n)
                {
                    return i;
                }
            }

            return -1;
        }

        public static int LineCount(this string text)
        {
            Verify.NotNull(text, nameof(text));

            int lineCount = 1;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    lineCount++;
                }
            }

            return lineCount;
        }
    }
}