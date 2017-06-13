using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    internal static class ParserRuleContextExtensions
    {
        public static bool HasStub(this ParserRuleContext context, Type stubType)
        {
            int childCount = context.ChildCount;
            return childCount > 0 && context.GetChild(childCount - 1).GetType() == stubType;
        }
    }
}