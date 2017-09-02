using System;
using System.Collections.Generic;
using System.Linq;
using Android.Text.Style;
using Repository.Common.Validation;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Android;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal static class TextColorerExtensions
    {
        public static IDisposable FlushEveryToken(this TextColorer colorer)
        {
            Verify.NotNull(colorer, nameof(colorer));

            return colorer.Setup(flushFrequency: 1);
        }

        public static IEnumerable<SyntaxAssignment> GetSyntaxAssignments(this TextColorer colorer)
        {
            Verify.NotNull(colorer, nameof(colorer));
            Verify.Argument(colorer.Theme == TestColorTheme.Instance, nameof(colorer));

            var text = colorer.Text;
            var rawText = text.ToString();
            var colorSpans = text.GetSpans<ForegroundColorSpan>().OrderBy(s => text.GetSpanStart(s));

            foreach (var span in colorSpans)
            {
                int spanStart = text.GetSpanStart(span);
                int spanCount = text.GetSpanEnd(span) - spanStart;

                var token = rawText.Substring(spanStart, spanCount);
                var kind = TestColorTheme.Decode(span.ForegroundColor);

                yield return (token, kind);
            }
        }
    }
}