using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.SyntaxHighlighting;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    internal struct TargetedKindOverride
    {
        private TargetedKindOverride(Type targetType, SyntaxKind kind)
        {
            Debug.Assert(typeof(IParseTree).IsAssignableFrom(targetType));
            Debug.Assert(kind.IsValid());

            TargetType = targetType;
            Kind = kind;
        }

        public static TargetedKindOverride Create<TTarget>(SyntaxKind kind)
            where TTarget : IParseTree
            => new TargetedKindOverride(typeof(TTarget), kind);

        public SyntaxKind Kind { get; }

        public Type TargetType { get; }
    }
}