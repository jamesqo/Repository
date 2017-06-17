using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Runtime;
using Android.Text;
using Java.Lang;
using Repository.Common;

namespace Repository.Internal.EditorServices.Highlighting
{
    internal class SpannableText : Java.Lang.Object, ISpannable
    {
        private struct SpanInfo
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
        }

        private readonly string _rawText;
        private readonly Dictionary<Java.Lang.Object, SpanInfo> _map;
        private readonly List<Java.Lang.Object> _spans;
        private readonly List<SpanInfo> _infos;

        private SpannableText(string rawText)
        {
            Verify.NotNull(rawText, nameof(rawText));

            _rawText = rawText;
            // Avoid calls to Java.Lang.Object.GetHashCode(), Java marshalling is slow.
            _map = new Dictionary<Java.Lang.Object, SpanInfo>(ReferenceEqualityComparer.Instance);
            _spans = new List<Java.Lang.Object>();
            _infos = new List<SpanInfo>();
        }

        public char CharAt(int index) => _rawText[index];

        public IEnumerator<char> GetEnumerator() => _rawText.GetEnumerator();

        public int GetSpanEnd(Java.Lang.Object tag)
            => _map.TryGetValue(tag, out var info) ? info.End : -1;

        [return: GeneratedEnum]
        public SpanTypes GetSpanFlags(Java.Lang.Object tag)
            => _map.TryGetValue(tag, out var info) ? info.Flags : default(SpanTypes);

        public Java.Lang.Object[] GetSpans(int start, int end, Class type)
        {
            var spans = new List<Java.Lang.Object>();

            for (int i = 0; i < _infos.Count; i++)
            {
                var info = _infos[i];
                var span = _spans[i];

                int spanStart = info.Start, spanEnd = info.End;
                if (end < spanStart || spanEnd < start)
                {
                    continue;
                }

                Debug.Assert(spanStart != spanEnd);
                if (start != end && (end == spanStart || spanEnd == start))
                {
                    continue;
                }

                // TODO: Missed out on optimization for `type` being Object.class.
                // See https://github.com/android/platform_frameworks_base/blob/b5c4e80ecd47dda8c73b0e93eb2ee1a8da58c981/core/java/android/text/SpannableStringInternal.java#L313
                if (type != null && !type.IsInstance(span))
                {
                    continue;
                }

                spans.Add(span);
            }
        }

        public int GetSpanStart(Java.Lang.Object tag)
            => _map.TryGetValue(tag, out var info) ? info.Start : -1;

        public int Length() => _rawText.Length;

        public int NextSpanTransition(int start, int limit, Class type)
        {
            throw new NotImplementedException();
        }

        public void RemoveSpan(Java.Lang.Object what) => throw new NotSupportedException();

        public void SetSpan(Java.Lang.Object what, int start, int end, [GeneratedEnum] SpanTypes flags)
        {
            var info = new SpanInfo(start, end, flags);
            _map.Add(what, info);
            _spans.Add(what);
            _infos.Add(info);
        }

        public ICharSequence SubSequenceFormatted(int start, int end)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}