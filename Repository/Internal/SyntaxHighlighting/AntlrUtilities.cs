using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace Repository.Internal.SyntaxHighlighting
{
    internal static class AntlrUtilities
    {
        public const int Eof = -1;

        public static IList<IToken> Tokenize(string text, Func<ICharStream, ITokenSource> lexerFactory)
        {
            Verify.NotNullOrEmpty(text, nameof(text));
            Verify.NotNull(lexerFactory, nameof(lexerFactory));

            var inputStream = new AntlrInputStream(text);
            var lexer = lexerFactory(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            tokenStream.Fill();
            return tokenStream.GetTokens();
        }
    }
}