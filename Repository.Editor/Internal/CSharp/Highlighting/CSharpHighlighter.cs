using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Repository.Editor.Highlighting;
using static Microsoft.CodeAnalysis.Classification.ClassificationTypeNames;

namespace Repository.Editor.Internal.CSharp.Highlighting
{
    internal class CSharpHighlighter : IHighlighter
    {
        private class Worker
        {
            private readonly string _sourceText;
            private readonly ITextColorer _colorer;
            private readonly IEnumerable<ClassifiedSpan> _spans;

            private int _index;

            internal Worker(string sourceText, ITextColorer colorer)
            {
                _sourceText = sourceText;
                _colorer = colorer;
                _spans = GetClassifiedSpans(sourceText);
            }

            internal void Run()
            {
                foreach (var span in _spans)
                {
                    var textSpan = span.TextSpan;
                    HandleSkippedText(_index, textSpan.Start);
                    Advance(textSpan.Length, GetSyntaxKind(span.ClassificationType));
                }
            }

            private void HandleSkippedText(int lastSpanEnd, int spanStart)
            {
                if (lastSpanEnd < spanStart)
                {
                    int skipped = spanStart - lastSpanEnd;
                    Advance(skipped, SyntaxKind.Plaintext);
                }
            }

            private void Advance(int count, SyntaxKind kind)
            {
                _colorer.Color(kind, count);
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
                // TODO: Add async support to IHighlighter? IsAsync, or just force everything to return a Task?
                // Probably won't be an issue after this class is rewritten to do manual classification.
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

        public void Highlight(string text, ITextColorer colorer) => new Worker(text, colorer).Run();
    }
}
