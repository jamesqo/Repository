using System.Collections.Generic;
using System.Linq;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal static class SyntaxAssignmentExtensions
    {
        public static IEnumerable<SyntaxAssignment> RemoveWhitespaceTokens(
            this IEnumerable<SyntaxAssignment> assignments)
        {
            Verify.NotNull(assignments, nameof(assignments));

            return assignments.Where(a => !string.IsNullOrWhiteSpace(a.Token));
        }
    }
}