using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            private readonly CancellationToken _cancellationToken;

            private int _index;

            internal Worker(string sourceText, ITextColorer colorer, CancellationToken cancellationToken)
            {
                _sourceText = sourceText;
                _colorer = colorer;
                _spans = GetClassifiedSpans(sourceText);
                _cancellationToken = cancellationToken;
            }

            internal async Task Run()
            {
                foreach (var span in _spans)
                {
                    var textSpan = span.TextSpan;
                    HandleSkippedText(_index, textSpan.Start);
                    await Advance(textSpan.Length, GetSyntaxKind(span.ClassificationType));
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

            private Task Advance(int count, SyntaxKind kind)
            {
                _index += count;
                return _colorer.Color(kind, count, _cancellationToken);
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
                return Classifier.GetClassifiedSpansAsync(document, new TextSpan(0, sourceText.Length)).Result;
            }

            private static SyntaxKind GetSyntaxKind(string classificationType)
            {
                switch (classificationType)
                {
                    case ClassName:
                        return SyntaxKind.TypeDeclaration;
                    case Comment:
                        return SyntaxKind.Comment;
                    case DelegateName:
                    case EnumName:
                        return SyntaxKind.TypeDeclaration;
                    case ExcludedCode:
                        return SyntaxKind.ExcludedCode;
                    case Identifier:
                        return SyntaxKind.Identifier;
                    case InterfaceName:
                        return SyntaxKind.TypeDeclaration;
                    case Keyword:
                        return SyntaxKind.Keyword;
                    case ModuleName:
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
                        return SyntaxKind.Plaintext;
                    case TypeParameterName:
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

        public Task Highlight(string text, ITextColorer colorer, CancellationToken cancellationToken)
        {
            return new Worker(text, colorer, cancellationToken).Run();
        }
    }
}
