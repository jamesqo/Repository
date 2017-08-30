using System;
using Antlr4.Runtime;
using Repository.Common.Validation;

namespace Repository.Editor.Internal.Common.Highlighting
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