using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Repository.EditorServices.Highlighting;
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

            }

            internal void Run() => Visit(_tree.GetRoot());

            private void Advance(int count, CommonSyntaxKind kind)
            {
                _colorer.Color(kind, _index, count);
                _index += count;
            }
        }

        public TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer)
        {
            new Visitor(text, colorer).Run();
            return colorer.Result;
        }
    }
}
