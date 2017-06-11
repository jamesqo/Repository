using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal.SyntaxHighlighting
{
    internal interface ISyntaxStyler
    {
        Java.Lang.Object GetSpan(SyntaxKind kind);
    }
}