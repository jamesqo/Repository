using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.EditorServices.SyntaxHighlighting;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct SyntaxKindSuggestion
    {
        private readonly int _value;

        private SyntaxKindSuggestion(int value) => _value = value;

        public static implicit operator SyntaxKindSuggestion(SyntaxKind kind)
        {
            Debug.Assert(kind.IsValid() && !kind.IsNone());
            return new SyntaxKindSuggestion((int)kind);
        }

        public static SyntaxKindSuggestion Overridable(SyntaxKind kind)
        {
            Debug.Assert(kind.IsValid() && !kind.IsNone());
            return new SyntaxKindSuggestion((int)kind | int.MinValue);
        }

        public bool IsOverridable => _value < 0;

        public SyntaxKind Kind => (SyntaxKind)(_value & int.MaxValue);

        private string DebuggerDisplay => $"{nameof(Kind)} = {Kind}, {nameof(IsOverridable)} = {IsOverridable}";

        public SyntaxKind TryOverride(SyntaxKind @override)
        {
            Debug.Assert(@override.IsValid());
            return IsOverridable && !@override.IsNone() ? @override : Kind;
        }
    }
}