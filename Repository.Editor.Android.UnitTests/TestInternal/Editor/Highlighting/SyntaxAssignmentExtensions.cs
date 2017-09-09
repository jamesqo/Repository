using System.Collections.Generic;
using System.Linq;
using Repository.Common;
using Repository.Common.Validation;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal static class SyntaxAssignmentExtensions
    {
        public static IEnumerable<SyntaxAssignment> RemoveConsecutiveTokens(
            this IEnumerable<SyntaxAssignment> assignments,
            IEnumerable<string> tokens,
            out int index)
        {
            Verify.NotNullOrEmpty(assignments, nameof(assignments));
            Verify.NotNullOrEmpty(tokens, nameof(tokens));

            var allTokens = assignments.Select(a => a.Token);
            index = allTokens.IndexOf(tokens);

            Verify.ValidState(
                index != -1,
                $"{nameof(tokens)}: {Dumper.ToString(tokens)} is not a sublist of {nameof(allTokens)}: {Dumper.ToString(allTokens)}.");

            return allTokens.Take(index).Concat(allTokens.Skip(index + tokens.Count()));
        }

        public static IEnumerable<SyntaxAssignment> RemoveWhitespaceTokens(
            this IEnumerable<SyntaxAssignment> assignments)
        {
            Verify.NotNull(assignments, nameof(assignments));

            return assignments.Where(a => !string.IsNullOrWhiteSpace(a.Token));
        }
    }
}