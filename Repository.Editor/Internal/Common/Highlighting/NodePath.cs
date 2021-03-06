﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Tree;
using Repository.Common;
using Repository.Common.Validation;

namespace Repository.Editor.Internal.Common.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct NodePath : IEquatable<NodePath>
    {
        private static ImmutableArray<string> TrimFromDisplayNames { get; } =
            ImmutableArray.Create("Context", "NodeImpl");

        private readonly ImmutableSpan<Type> _nodeTypes;

        private NodePath(ImmutableSpan<Type> nodeTypes)
        {
            Verify.Argument(!nodeTypes.IsDefaultOrEmpty, nameof(nodeTypes));

            _nodeTypes = nodeTypes;
        }

        public static NodePath Create(params Type[] nodeTypes) => Create(ImmutableArray.Create(nodeTypes));

        public static NodePath Create(ImmutableArray<Type> nodeTypes) => new NodePath(nodeTypes);

        public static NodePath GetRelativePath(IParseTree ancestor, IParseTree descendant)
        {
            Debug.Assert(ancestor != descendant);
            Debug.Assert(ancestor.HasDescendant(descendant));

            var builder = ImmutableArray.CreateBuilder<Type>();
            var current = descendant;
            do
            {
                builder.Add(current.GetType());
                current = current.Parent;
            }
            while (current != ancestor);

            builder.Reverse();
            return Create(builder.ToImmutable());
        }

        public bool IsDefault => _nodeTypes.IsDefault;

        public int Length => _nodeTypes.Count;

        internal string DebuggerDisplay => "/" + string.Join("/", _nodeTypes.Select(GetDisplayName));

        public override bool Equals(object obj) => obj is NodePath other && Equals(other);

        public bool Equals(NodePath other) => Length == other.Length && StartsWith(other);

        public override int GetHashCode() => throw new NotSupportedException();

        public bool StartsWith(NodePath other) => _nodeTypes.StartsWithReferences(other._nodeTypes);

        public NodePath Subpath(int index) => new NodePath(_nodeTypes.Slice(index));

        public bool TryChangeRoot(NodePath root, out NodePath newPath)
        {
            if (StartsWith(root))
            {
                newPath = Subpath(root.Length);
                return true;
            }

            newPath = default;
            return false;
        }

        private static string GetDisplayName(Type nodeType)
        {
            var name = nodeType.Name;
            foreach (var suffix in TrimFromDisplayNames)
            {
                if (name.EndsWithOrdinal(suffix))
                {
                    name = name.Substring(0, name.Length - suffix.Length);
                }
            }

            return name;
        }
    }
}