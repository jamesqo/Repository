using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Text;

namespace Repository.Internal.SyntaxHighlighting
{
    internal interface ISyntaxHighlighter
    {
        SpannableString Highlight(string text, ISyntaxStyler styler);
    }
}