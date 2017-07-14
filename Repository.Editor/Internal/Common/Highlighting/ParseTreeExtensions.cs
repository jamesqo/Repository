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

            var current = descendant;
            do
            {
                current = current.Parent;
                if (ancestor == current)
                {
                    return true;
                }
            }
            while (current != null);

            return false;
        }
    }
}