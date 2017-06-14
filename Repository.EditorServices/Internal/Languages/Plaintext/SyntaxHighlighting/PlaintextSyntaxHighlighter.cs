using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Text;
using Repository.EditorServices.SyntaxHighlighting;

namespace Repository.EditorServices.Internal.Languages.Plaintext.SyntaxHighlighting
{
    internal class PlaintextSyntaxHighlighter : ISyntaxHighlighter
    {
        public SpannableString Highlight(string text, ISyntaxStyler styler)
        {
            var span = styler.GetSpan(SyntaxKind.Identifier);
            var result = new SpannableString(text);
            result.SetSpan(span, 0, text.Length, SpanTypes.InclusiveExclusive);
            return result;
        }
    }
}