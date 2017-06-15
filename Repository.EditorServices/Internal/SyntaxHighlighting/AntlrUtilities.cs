using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Repository.Internal;

namespace Repository.EditorServices.Internal.SyntaxHighlighting
{
    internal static class AntlrUtilities
    {
        public static CommonTokenStream TokenStream(string text, Func<ICharStream, ITokenSource> lexerFactory)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(lexerFactory, nameof(lexerFactory));

            var inputStream = new AntlrInputStream(text);
            var lexer = lexerFactory(inputStream);
            return new CommonTokenStream(lexer);
        }
    }
}