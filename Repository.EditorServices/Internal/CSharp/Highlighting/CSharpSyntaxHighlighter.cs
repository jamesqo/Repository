using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Repository.EditorServices.Highlighting;
using Repository.EditorServices.Internal.Common.Highlighting;
using CommonSyntaxKind = Repository.EditorServices.Highlighting.SyntaxKind;

namespace Repository.EditorServices.Internal.CSharp.Highlighting
{
    internal class CSharpSyntaxHighlighter : ISyntaxHighlighter
    {
        private class Visitor : CSharpSyntaxVisitor
        {
            private readonly SyntaxTree _tree;
            private readonly ISyntaxColorer _colorer;

            private int _index;

            internal Visitor(string text, ISyntaxColorer colorer)
            {
                _tree = CSharpSyntaxTree.ParseText(text);
                _colorer = colorer;
            }

            public override void DefaultVisit(SyntaxNode node)
            {
                foreach (var nodeOrToken in node.ChildNodesAndTokens())
                {
                    if (nodeOrToken.IsToken)
                    {
                        VisitToken(nodeOrToken.AsToken());
                    }

                    Visit(nodeOrToken.AsNode());
                }
            }

            internal void Run() => Visit(_tree.GetRoot());

            private void Advance(int count, CommonSyntaxKind kind)
            {
                _colorer.Color(kind, _index, count);
                _index += count;
            }

            private SyntaxReplacement FindReplacement(SyntaxToken token)
            {

            }

            private SyntaxSuggestion SuggestCommonKind(SyntaxToken token)
            {
                switch (token.Kind())
                {

                }
            }

            private void VisitTrivia(SyntaxTrivia trivia)
            {
                // TODO: Handle 'structured trivia'?
                int count = trivia.ToString().Length;
                var replacementKind = FindReplacement(trivia).Kind;
                Advance(count, replacementKind);
            }

            private void VisitTriviaList(SyntaxTriviaList list)
            {
                foreach (var trivia in list)
                {
                    VisitTrivia(trivia);
                }
            }

            private void VisitToken(SyntaxToken token)
            {
                VisitTriviaList(token.LeadingTrivia);
                VisitTokenCore(token);
                VisitTriviaList(token.TrailingTrivia);
            }

            private void VisitTokenCore(SyntaxToken token)
            {
                int count = token.Text.Length;
                var replacementKind = FindReplacement(token).Kind;
                Advance(count, SuggestCommonKind(token).TryReplace(replacementKind));
            }
        }

        public TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer)
        {
            new Visitor(text, colorer).Run();
            return colorer.Result;
        }
    }
}
