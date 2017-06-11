using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Repository.Internal.SyntaxHighlighting
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