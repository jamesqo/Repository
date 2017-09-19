using System;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal struct SyntaxAssignment : IEquatable<SyntaxAssignment>
    {
        private SyntaxAssignment(string token, SyntaxKind kind)
        {
            Verify.NotNullOrEmpty(token, nameof(token));
            Verify.Argument(kind.IsValid(), nameof(kind));

            Token = token;
            Kind = kind;
        }

        public bool IsDefault => Token == null;

        public SyntaxKind Kind { get; }

        public string Token { get; }

        public static implicit operator SyntaxAssignment((string, SyntaxKind) tuple)
        {
            return new SyntaxAssignment(tuple.Item1, tuple.Item2);
        }

        public override bool Equals(object obj)
            => obj is SyntaxAssignment other && Equals(other);

        public bool Equals(SyntaxAssignment other)
            => Token == other.Token
            && Kind == other.Kind;

        public override int GetHashCode() => throw new NotSupportedException();
    }
}