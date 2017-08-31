using System.Collections.Generic;
using System.Linq;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<(string token, SyntaxKind kind)> RemoveWhitespaceTokens(
            this IEnumerable<(string token, SyntaxKind kind)> assignments)
        {
            Verify.NotNull(assignments, nameof(assignments));

            return assignments.Where(a => !string.IsNullOrWhiteSpace(a.token));
        }
    }
}