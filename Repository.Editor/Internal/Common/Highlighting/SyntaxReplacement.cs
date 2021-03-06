﻿using System.Diagnostics;
using Antlr4.Runtime.Tree;
using Repository.Common;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Internal.Common.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct SyntaxReplacement
    {
        public static SyntaxReplacement None => default;

        private SyntaxReplacement(NodePath path, SyntaxKind kind)
        {
            Verify.Argument(!path.IsDefault, nameof(path));
            Verify.Argument(kind.IsValid(), nameof(kind));

            Path = path;
            Kind = kind;
        }

        public static SyntaxReplacement Create<TChild>(SyntaxKind kind)
            where TChild : class, IParseTree
            => Create(NodePath.Create(typeof(TChild)), kind);

        public static SyntaxReplacement Create(NodePath path, SyntaxKind kind) => new SyntaxReplacement(path, kind);

        public static SyntaxReplacement Terminal(SyntaxKind kind) => Create<TerminalNodeImpl>(kind);

        public SyntaxKind Kind { get; }

        public NodePath Path { get; }

        private string DebuggerDisplay => $"{nameof(Kind)} = {Kind}, {nameof(Path)} = {Path.DebuggerDisplay}";

        public SyntaxReplacement WithPath(NodePath path) => Create(path, Kind);
    }
}