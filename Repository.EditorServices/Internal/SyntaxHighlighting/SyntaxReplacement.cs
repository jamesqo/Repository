﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.SyntaxHighlighting;
using Repository.Common;

namespace Repository.EditorServices.Internal.SyntaxHighlighting
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct SyntaxReplacement
    {
        public static SyntaxReplacement None => default(SyntaxReplacement);

        private SyntaxReplacement(NodePath path, SyntaxKind kind)
        {
            Verify.NotNull(path, nameof(path));
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