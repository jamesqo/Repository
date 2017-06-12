using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.SyntaxHighlighting;
using Repository.Internal.EditorServices.SyntaxHighlighting.Grammars;
using static Repository.Internal.EditorServices.SyntaxHighlighting.Grammars.JavaParser;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    internal class JavaSyntaxHighlighter : ISyntaxHighlighter
    {
        private class Visitor : JavaBaseVisitor<object>
        {
            private readonly string _rawText;
            private readonly SpannableString _text;
            private readonly ISyntaxStyler _styler;
            private readonly CommonTokenStream _stream;

            private int _index;
            private int _lastTokenIndex;
            private SyntaxKind _defaultKind;

            internal Visitor(string text, ISyntaxStyler styler)
            {
                _rawText = text;
                _text = new SpannableString(text);
                _styler = styler;
                _stream = AntlrUtilities.TokenStream(text, input => new JavaLexer(input));
                _lastTokenIndex = -1;
                _defaultKind = SyntaxKind.None;
            }

            public override object VisitAnnotation([NotNull] AnnotationContext context)
            {
                var defaultKind = _defaultKind;
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    switch (child)
                    {
                        case ITerminalNode node:
                            Advance(node, defaultKind);
                            break;
                        default:
                            Visit(child);
                            break;
                    }
                }

                return null;
            }

            public override object VisitAnnotationName([NotNull] AnnotationNameContext context)
            {
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    switch (child)
                    {
                        case QualifiedNameContext qualifiedName:
                            _defaultKind = SyntaxKind.Annotation;
                            VisitQualifiedName(qualifiedName);
                            break;
                        default:
                            Visit(child);
                            break;
                    }
                }

                return null;
            }

            public override object VisitClassDeclaration([NotNull] ClassDeclarationContext context)
            {
                var defaultKind = _defaultKind;
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    switch (child)
                    {
                        case ITerminalNode node:
                            Advance(node, defaultKind);
                            break;
                        default:
                            Visit(child);
                            break;
                    }
                }

                return null;
            }

            public override object VisitClassOrInterfaceModifier([NotNull] ClassOrInterfaceModifierContext context)
            {
                var defaultKind = _defaultKind;
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    switch (child)
                    {
                        case ITerminalNode node:
                            Advance(node, defaultKind);
                            break;
                        default:
                            Visit(child);
                            break;
                    }
                }

                return null;
            }

            public override object VisitPackageDeclaration([NotNull] PackageDeclarationContext context)
            {
                var defaultKind = _defaultKind;
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    switch (child)
                    {
                        case ITerminalNode node:
                            Advance(node, defaultKind);
                            break;
                        default:
                            Visit(child);
                            break;
                    }
                }

                return null;
            }

            public override object VisitImportDeclaration([NotNull] ImportDeclarationContext context)
            {
                var defaultKind = _defaultKind;
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    switch (child)
                    {
                        case ITerminalNode node:
                            Advance(node, defaultKind);
                            break;
                        default:
                            Visit(child);
                            break;
                    }
                }

                return null;
            }

            public override object VisitTerminal(ITerminalNode node)
            {
                Advance(node, _defaultKind);
                return null;
            }

            internal SpannableString HighlightText()
            {
                Visit(CreateTree());
                return _text;
            }

            private void Advance(IToken token, SyntaxKind kind)
            {
                Approach(token);
                Surpass(token, kind);
            }

            private void Advance(ITerminalNode node, SyntaxKind defaultKind)
            {
                var token = node.Symbol;
                var kind = SuggestKind(token).TryOverride(defaultKind);
                Advance(token, kind);
            }

            private void Approach(IToken token)
            {
                int startTokenIndex = _lastTokenIndex + 1;
                int endTokenIndex = token.TokenIndex;

                for (int i = startTokenIndex; i < endTokenIndex; i++)
                {
                    SurpassHidden(_stream.Get(i));
                }
            }

            private CompilationUnitContext CreateTree() => new JavaParser(_stream).compilationUnit();

            private SyntaxKind GetHiddenKind(IToken token)
            {
                switch (token.Type)
                {
                    case WS:
                        // TODO
                        return SyntaxKind.Identifier;
                    case COMMENT:
                    case LINE_COMMENT:
                        return SyntaxKind.Comment;
                    default:
                        throw new NotSupportedException();
                }
            }

            private SyntaxKindSuggestion SuggestKind(IToken token)
            {
                // TODO: No need to handle hidden token types below. They'll never get run.

                switch (token.Type)
                {
                    case ABSTRACT: return SyntaxKind.Keyword;
                    // TODO and all.
                }
            }

            private void Surpass(IToken token, SyntaxKind kind)
            {
                _lastTokenIndex++;
                Debug.Assert(_lastTokenIndex == token.TokenIndex);

                int count = token.Text.Length;
                var span = _styler.GetSpan(kind);
                _text.SetSpan(span, _index, _index + count, SpanTypes.InclusiveExclusive);
                _index += count;
            }

            private void SurpassHidden(IToken token)
            {
                var kind = GetHiddenKind(token);
                Surpass(token, kind);
            }
        }

        public SpannableString Highlight(string text, ISyntaxStyler styler) => new Visitor(text, styler).HighlightText();
    }
}