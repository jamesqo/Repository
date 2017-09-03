using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.Common;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Internal.Common.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct SyntaxSuggestion
    {
        private readonly int _value;

        private SyntaxSuggestion(int value) => _value = value;

        public static implicit operator SyntaxSuggestion(SyntaxKind kind)
        {
            Verify.Argument(kind.IsValid() && !kind.IsNone(), nameof(kind));
            return new SyntaxSuggestion((int)kind);
        }

        public static SyntaxSuggestion Replaceable(SyntaxKind kind)
        {
            Verify.Argument(kind.IsValid() && !kind.IsNone(), nameof(kind));
            return new SyntaxSuggestion((int)kind | int.MinValue);
        }

        public bool IsReplaceable => _value < 0;

        public SyntaxKind Kind => (SyntaxKind)(_value & int.MaxValue);

        private string DebuggerDisplay => $"{nameof(Kind)} = {Kind}, {nameof(IsReplaceable)} = {IsReplaceable}";

        public SyntaxKind TryReplace(SyntaxKind replacement)
        {
            Verify.Argument(replacement.IsValid(), nameof(replacement));
            return IsReplaceable && !replacement.IsNone() ? replacement : Kind;
        }
    }
}