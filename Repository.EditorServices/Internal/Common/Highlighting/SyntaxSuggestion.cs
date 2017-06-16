using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.EditorServices.Highlighting;

namespace Repository.EditorServices.Internal.Common.Highlighting
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct SyntaxSuggestion
    {
        private readonly int _value;

        private SyntaxSuggestion(int value) => _value = value;

        public static implicit operator SyntaxSuggestion(SyntaxKind kind)
        {
            Debug.Assert(kind.IsValid() && !kind.IsNone());
            return new SyntaxSuggestion((int)kind);
        }

        public static SyntaxSuggestion Replaceable(SyntaxKind kind)
        {
            Debug.Assert(kind.IsValid() && !kind.IsNone());
            return new SyntaxSuggestion((int)kind | int.MinValue);
        }

        public bool IsReplaceable => _value < 0;

        public SyntaxKind Kind => (SyntaxKind)(_value & int.MaxValue);

        private string DebuggerDisplay => $"{nameof(Kind)} = {Kind}, {nameof(IsReplaceable)} = {IsReplaceable}";

        public SyntaxKind TryReplace(SyntaxKind replacement)
        {
            Debug.Assert(replacement.IsValid());
            return IsReplaceable && !replacement.IsNone() ? replacement : Kind;
        }
    }
}