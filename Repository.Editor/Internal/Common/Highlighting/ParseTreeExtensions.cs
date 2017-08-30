using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace Repository.Editor.Internal.Common.Highlighting
{
    internal static class ParseTreeExtensions
    {
        public static bool HasDescendant(this IParseTree ancestor, IParseTree descendant)
        {
            Debug.Assert(ancestor != null);
            Debug.Assert(descendant != null);

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