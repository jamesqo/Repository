using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using Repository.Common.Validation;

namespace Repository.Editor.Internal.Common.Highlighting
{
    internal static class ParseTreeExtensions
    {
        public static bool HasDescendant(this IParseTree ancestor, IParseTree descendant)
        {
            Verify.NotNull(ancestor, nameof(ancestor));
            Verify.NotNull(descendant, nameof(descendant));

            var current = descendant.Parent;
            while (current != null)
            {
                if (ancestor == current)
                {
                    return true;
                }
                current = current.Parent;
            }

            return false;
        }
    }
}