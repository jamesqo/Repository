using Antlr4.Runtime;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Internal.Common.Highlighting
{
    internal interface IAntlrSyntaxProvider
    {
        SyntaxSuggestion SuggestKind(IToken token);
    }

    internal static class AntlrSyntaxProviderExtensions
    {
        public static SyntaxKind GetKind(this IAntlrSyntaxProvider syntaxProvider, IToken token)
        {
            Verify.NotNull(syntaxProvider, nameof(syntaxProvider));

            return syntaxProvider.SuggestKind(token).Kind;
        }
    }
}
