using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.EditorServices.SyntaxHighlighting
{
    public interface ISyntaxStyler
    {
        Java.Lang.Object GetSpan(SyntaxKind kind);
    }
}