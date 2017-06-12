using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    internal static class AbstractParseTreeVisitorExtensions
    {
        // TODO: Takie into account DefaultResult, ShouldVisitNextChild, AggregateResult and return something somehow?
        public static void VisitChildren<Result>(this AbstractParseTreeVisitor<Result> visitor, IRuleNode node, int startIndex)
        {
            visitor.VisitChildren(node, startIndex, node.ChildCount - startIndex);
        }

        public static void VisitChildren<Result>(this AbstractParseTreeVisitor<Result> visitor, IRuleNode node, int startIndex, int count)
        {
            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                visitor.Visit(node.GetChild(i));
            }
        }
    }
}