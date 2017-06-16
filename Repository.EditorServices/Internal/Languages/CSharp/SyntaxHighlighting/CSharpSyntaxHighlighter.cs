﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Repository.EditorServices.SyntaxHighlighting;
using static Microsoft.CodeAnalysis.Classification.ClassificationTypeNames;

namespace Repository.EditorServices.Internal.Languages.CSharp.SyntaxHighlighting
{
    internal class CSharpSyntaxHighlighter : ISyntaxHighlighter
    {
        private class Worker
        {
            private readonly string _sourceText;
            private readonly ISyntaxColorer _colorer;
            private readonly IEnumerator<ClassifiedSpan> _spanEnumerator;

            private int _index;
            // TODO: Rename _lastTokenIndex to _lastTokenEnd in JavaSyntaxHighlighter? Shift up its value 1? (This follows the behavior of TextSpan.End?)

            internal Worker(string sourceText, ISyntaxColorer colorer)
            {
                _sourceText = sourceText;
                _colorer = colorer;
                _spanEnumerator = GetClassifiedSpans(sourceText).GetEnumerator();
            }

            internal void Run()
            {
                while (_spanEnumerator.MoveNext())
                {
                    var current = _spanEnumerator.Current;
                    var textSpan = current.TextSpan;

                    var spanStart = textSpan.Start;
                    int lastSpanEnd = _index;

                    if (lastSpanEnd < spanStart)
                    {
                        int skipped = spanStart - lastSpanEnd;
                        Advance(skipped, SyntaxKind.Plaintext);
                    }

                    Advance(textSpan.Length, GetSyntaxKind(current.ClassificationType));
                }
            }

            private void Advance(int count, SyntaxKind kind)
            {
                _colorer.Color(kind, _index, count);
                _index += count;
            }

            private static Document CreateDocument(string sourceText)
            {
                var workspace = new AdhocWorkspace();
                var solution = workspace.CurrentSolution;
                var project = solution.AddProject(string.Empty, string.Empty, LanguageNames.CSharp);
                return project.AddDocument(string.Empty, sourceText);
            }

            private static IEnumerable<ClassifiedSpan> GetClassifiedSpans(string sourceText)
            {
                var document = CreateDocument(sourceText);
                // TODO: Add async support to ISyntaxHighlighter? IsAsync, or just force everything to return a Task?
                return Classifier.GetClassifiedSpansAsync(document, new TextSpan(0, sourceText.Length)).Result;
            }

            private static SyntaxKind GetSyntaxKind(string classificationType)
            {
                switch (classificationType)
                {
                    case ClassName:
                        // TODO: The C# APIs don't offer distinction between type declarations and type idents like ANTLR does.
                        // What should be our behavior here?
                        return SyntaxKind.TypeDeclaration;
                    case Comment:
                        return SyntaxKind.Comment;
                    case DelegateName:
                    case EnumName:
                        return SyntaxKind.TypeDeclaration;
                    // TODO: Consider this more carefully. What if all #if ...s get excluded?
                    case ExcludedCode:
                        // TODO: Implement in ColorTheme
                        return SyntaxKind.ExcludedCode;
                    case Identifier:
                        return SyntaxKind.Identifier;
                    case InterfaceName:
                        return SyntaxKind.TypeDeclaration;
                    case Keyword:
                        return SyntaxKind.Keyword;
                    case ModuleName:
                        // TODO: What is this?
                        throw new NotSupportedException();
                    case NumericLiteral:
                        return SyntaxKind.NumericLiteral;
                    case Operator:
                        return SyntaxKind.Operator;
                    case PreprocessorKeyword:
                        return SyntaxKind.Keyword;
                    case PreprocessorText:
                    case Punctuation:
                        return SyntaxKind.Plaintext;
                    case StringLiteral:
                        return SyntaxKind.StringLiteral;
                    case StructName:
                        return SyntaxKind.TypeDeclaration;
                    case Text:
                        // TODO: When is this actually hit? Is Plaintext appropriate?
                        return SyntaxKind.Plaintext;
                    case TypeParameterName:
                        // TODO: Is this the best choice? Should we add SyntaxKind.TypeParameterDeclaration?
                        return SyntaxKind.TypeIdentifier;
                    case VerbatimStringLiteral:
                        return SyntaxKind.StringLiteral;
                    case WhiteSpace:
                        return SyntaxKind.Plaintext;
                    case XmlDocCommentAttributeName:
                    case XmlDocCommentAttributeQuotes:
                    case XmlDocCommentAttributeValue:
                    case XmlDocCommentCDataSection:
                    case XmlDocCommentComment:
                    case XmlDocCommentDelimiter:
                    case XmlDocCommentEntityReference:
                    case XmlDocCommentName:
                    case XmlDocCommentProcessingInstruction:
                    case XmlDocCommentText:
                        return SyntaxKind.Comment;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer)
        {
            new Worker(text, colorer).Run();
            return colorer.Result;
        }
    }
}