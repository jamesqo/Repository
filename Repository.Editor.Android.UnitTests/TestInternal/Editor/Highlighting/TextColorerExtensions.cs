using System.Collections.Generic;
using Android.Text.Style;
using Repository.Common.Validation;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Android;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal static class TextColorerExtensions
    {
        public static IEnumerable<(string token, SyntaxKind kind)> GetSyntaxAssignments(this TextColorer colorer, bool skipWhitespaceTokens = true)
        {
            Verify.NotNull(colorer, nameof(colorer));
            Verify.Argument(colorer.Theme == TestColorTheme.Instance, nameof(colorer));

            var text = colorer.Text;
            var rawText = text.ToString();
            var colorSpans = text.GetSpans<ForegroundColorSpan>();

            foreach (var span in colorSpans)
            {
                int spanStart = text.GetSpanStart(span);
                int spanCount = text.GetSpanEnd(span) - spanStart;

                var token = rawText.Substring(spanStart, spanCount);
                var kind = TestColorTheme.Decode(span.ForegroundColor);

                if (!skipWhitespaceTokens || !string.IsNullOrWhiteSpace(token))
                {
                    yield return (token, kind);
                }
            }
        }
    }
}