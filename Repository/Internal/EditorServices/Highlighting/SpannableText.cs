using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.Runtime;
using Android.Text;
using Java.Lang;
using Repository.Common;
using JavaObject = Java.Lang.Object;

namespace Repository.Internal.EditorServices.Highlighting
{
    // TODO: This had to be public because of SyntaxColorer.Result. Move to non-internal ns?
    public class SpannableText : JavaObject, ISpannable
    {
        private struct SpanInfo : IComparable<SpanInfo>
        {
            public SpanInfo(int start, int end, SpanTypes flags)
            {
                Start = start;
                End = end;
                Flags = flags;
            }

            public int End { get; }

            public SpanTypes Flags { get; }

            public int Start { get; }

            // TODO: Use this
            public int CompareTo(SpanInfo other)
            {
                Debug.Assert(Start != other.Start);
                return Start - other.Start;
            }
        }

        private readonly string _rawText;
        private readonly Dictionary<JavaObject, SpanInfo> _map;
        private readonly List<JavaObject> _spans;
        private readonly List<SpanInfo> _infos;

        private SpannableText(string rawText)
        {
            Verify.NotNull(rawText, nameof(rawText));

            _rawText = rawText;
            // Avoid calls to Java.Lang.Object.GetHashCode(), Java marshalling is slow.
            _map = new Dictionary<JavaObject, SpanInfo>(ReferenceEqualityComparer.Instance);
            _infos = new List<SpanInfo>();
            _spans = new List<JavaObject>();
        }

        public static SpannableText Create(string rawText) => new SpannableText(rawText);

        private int SpanCount => _spans.Count;

        public char CharAt(int index) => _rawText[index];

        public IEnumerator<char> GetEnumerator() => _rawText.GetEnumerator();

        public int GetSpanEnd(JavaObject tag)
            => _map.TryGetValue(tag, out var info) ? info.End : -1;

        [return: GeneratedEnum]
        public SpanTypes GetSpanFlags(JavaObject tag)
            => _map.TryGetValue(tag, out var info) ? info.Flags : default(SpanTypes);

        public JavaObject[] GetSpans(int start, int end, Class type)
        {
            bool IsOverlapping(SpanInfo info)
            {
                int spanStart = info.Start, spanEnd = info.End;
                if (end < spanStart || spanEnd < start)
                {
                    return false;
                }

                Debug.Assert(spanStart != spanEnd);
                return start == end || (end != spanStart && spanEnd != start);
            }

            int CompareSpans(JavaObject span1, JavaObject span2)
            {
                return _map[span1].Flags - _map[span2].Flags;
            }

            // TODO: Missed out on optimization for `type` being Object.class.
            // See https://github.com/android/platform_frameworks_base/blob/b5c4e80ecd47dda8c73b0e93eb2ee1a8da58c981/core/java/android/text/SpannableStringInternal.java#L313
            bool IsPermitted(JavaObject span) => type == null || type.IsInstance(span);

            var spans = new SortedSet<JavaObject>(Comparer<JavaObject>.Create(CompareSpans));

            for (int i = 0; i < SpanCount; i++)
            {
                var info = _infos[i];
                var span = _spans[i];

                if (!IsOverlapping(info) || !IsPermitted(span))
                {
                    continue;
                }

                spans.Add(span);
            }

            return spans.ToArray();
        }

        public int GetSpanStart(JavaObject tag)
            => _map.TryGetValue(tag, out var info) ? info.Start : -1;

        public int Length() => _rawText.Length;

        public int NextSpanTransition(int start, int limit, Class type)
        {
            // TODO: Missed out on optimization for `type` being Object.class.
            // See https://github.com/android/platform_frameworks_base/blob/b5c4e80ecd47dda8c73b0e93eb2ee1a8da58c981/core/java/android/text/SpannableStringInternal.java#L313
            bool IsPermitted(JavaObject span) => type == null || type.IsInstance(span);

            bool TrySetTransition(int transition, JavaObject span, out bool @break)
            {
                if (limit <= transition)
                {
                    @break = true;
                    return false;
                }

                @break = false;

                if (start < transition && IsPermitted(span))
                {
                    limit = transition;
                    return true;
                }

                return false;
            }

            for (int i = 0; i < SpanCount; i++)
            {
                var info = _infos[i];
                var span = _spans[i];

                bool @break;
                if ((!TrySetTransition(info.Start, span, out @break) && @break) ||
                    (!TrySetTransition(info.End, span, out @break) && @break))
                {
                    break;
                }
            }

            return limit;
        }

        public void RemoveSpan(JavaObject what) => throw new NotSupportedException();

        public void SetSpan(JavaObject what, int start, int end, [GeneratedEnum] SpanTypes flags)
        {
            var info = new SpanInfo(start, end, flags);
            _map.Add(what, info);
            _spans.Add(what);
            _infos.Add(info);
        }

        public ICharSequence SubSequenceFormatted(int start, int end) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}