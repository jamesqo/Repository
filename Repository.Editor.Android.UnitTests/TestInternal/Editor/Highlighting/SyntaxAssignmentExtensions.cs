using System.Collections.Generic;
using System.Linq;
using Repository.Common.Collections;
using Repository.Common.Validation;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal static class SyntaxAssignmentExtensions
    {
        public static IEnumerable<SyntaxAssignment> ReplaceToken(
            this IEnumerable<SyntaxAssignment> assignments,
            string oldToken,
            SyntaxAssignment replacement)
        {
            return assignments.ReplaceConsecutiveTokens(new[] { oldToken }, new[] { replacement });
        }

        public static IEnumerable<SyntaxAssignment> ReplaceConsecutiveTokens(
            this IEnumerable<SyntaxAssignment> assignments,
            IEnumerable<string> oldTokens,
            IEnumerable<SyntaxAssignment> replacements)
        {
            Verify.NotNullOrEmpty(assignments, nameof(assignments));
            Verify.NotNullOrEmpty(oldTokens, nameof(oldTokens));
            Verify.NotNullOrEmpty(replacements, nameof(replacements));

            var allTokens = assignments.Select(a => a.Token);
            int index = allTokens.IndexOf(oldTokens);
            Verify.ValidState(index != -1, $"{nameof(oldTokens)} is not a sublist of {nameof(allTokens)}!");

            var before = assignments.Take(index);
            var after = assignments.Skip(index + oldTokens.Count());
            return before.Concat(replacements).Concat(after);
        }

        public static IEnumerable<SyntaxAssignment> RemoveWhitespaceTokens(
            this IEnumerable<SyntaxAssignment> assignments)
        {
            Verify.NotNull(assignments, nameof(assignments));

            return assignments.Where(a => !string.IsNullOrWhiteSpace(a.Token));
        }
    }
}