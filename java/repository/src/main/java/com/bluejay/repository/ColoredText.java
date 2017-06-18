package com.bluejay.repository;

import android.text.Spannable;
import android.text.style.ForegroundColorSpan;

import java.lang.reflect.Array;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class ColoredText implements Spannable {
    private final String rawText;
    private final HashMap<Object, SpanInfo> map;

    public ColoredText(String rawText, FragmentedReadStream colorings) {
        assert rawText != null;
        assert colorings != null;

        this.rawText = rawText;
        this.map = buildMap(colorings);
    }

    private static HashMap<Object, SpanInfo> buildMap(FragmentedReadStream colorings) {
        int coloringCount = colorings.byteCount() / 8;
        HashMap<Object, SpanInfo> map = new HashMap<>(coloringCount);
        int spanStart = 0;

        while (colorings.hasMore()) {
            long coloring = colorings.readLong();
            int color = getColor(coloring);
            int count = getCount(coloring);

            int spanEnd = spanStart + count;
            Object span = new ForegroundColorSpan(color);
            map.put(span, new SpanInfo(spanStart, spanEnd));
            spanStart = spanEnd;
        }

        return map;
    }

    private static int getColor(long coloring) {
        return (int)(coloring >>> 32);
    }

    private static int getCount(long coloring) {
        return (int)coloring;
    }

    @Override
    public void setSpan(Object o, int i, int i1, int i2) {
        throw new UnsupportedOperationException();
    }

    @Override
    public void removeSpan(Object o) {
        throw new UnsupportedOperationException();
    }

    @Override
    public <T> T[] getSpans(int i, int i1, Class<T> aClass) {
        int start = i, end = i1;
        Class<T> kind = aClass;

        // SpannableString has to worry about sorting the spans based on the SPAN_PRIORITY mask.
        // However, in our case we always use SPAN_INCLUSIVE_EXCLUSIVE, so every span has the same priority.
        // It's okay to leave them in the same order they were inserted.
        List<T> spans = new ArrayList<>();

        // Hack: This is an implementation detail, but as long as you don't remove items from it, a HashMap
        // will hand out entries in the same order you inserted them.
        for (HashMap.Entry<Object, SpanInfo> entry : this.map.entrySet()) {
            SpanInfo info = entry.getValue();
            int spanStart = info.getStart(), spanEnd = info.getEnd();

            if (end < spanStart || spanEnd < start) {
                continue;
            }

            assert spanStart != spanEnd;
            if (start != end && (end == spanStart || spanEnd == start)) {
                continue;
            }

            // Defer calling Class.isInstance as much as possible, since that's expensive.
            Object span = entry.getKey();
            if (kind != null && kind != Object.class && !kind.isInstance(span)) {
                continue;
            }

            spans.add((T) span);
        }

        return spans.toArray((T[]) Array.newInstance(kind, spans.size()));
    }

    @Override
    public int getSpanStart(Object o) {
        SpanInfo info = this.map.get(o);
        return info != null ? info.getStart() : -1;
    }

    @Override
    public int getSpanEnd(Object o) {
        SpanInfo info = this.map.get(o);
        return info != null ? info.getEnd() : -1;
    }

    @Override
    public int getSpanFlags(Object o) {
        return this.map.containsKey(o) ? SPAN_INCLUSIVE_EXCLUSIVE : 0;
    }

    @Override
    public int nextSpanTransition(int i, int i1, Class aClass) {
        throw new UnsupportedOperationException();
    }

    @Override
    public int length() {
        return this.rawText.length();
    }

    @Override
    public char charAt(int i) {
        return this.rawText.charAt(i);
    }

    @Override
    public CharSequence subSequence(int i, int i1) {
        throw new UnsupportedOperationException();
    }
}
