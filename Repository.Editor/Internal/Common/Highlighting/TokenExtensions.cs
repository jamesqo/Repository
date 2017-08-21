using Antlr4.Runtime;
using Repository.Common;

namespace Repository.Editor.Internal.Common.Highlighting
{
    internal static class TokenExtensions
    {
        /// <summary>
        /// Returns a value indicating whether <see cref="token"/> is an invalid token;
        /// that is, a token without any text or location in source code.
        /// </summary>
        /// <param name="token">The token.</param>
        public static bool IsInvalid(this IToken token)
        {
            Verify.NotNull(token, nameof(token));

            return token.TokenIndex == -1;
        }
    }
}
