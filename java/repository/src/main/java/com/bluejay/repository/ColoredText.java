package com.bluejay.repository;

import android.text.Editable;
import android.text.InputFilter;
import android.text.SpannableStringBuilder;
import android.text.style.*;

import java.util.Arrays;
import java.util.HashSet;
import java.util.Set;

public class ColoredText implements Editable {
    private static final Set<Class> noSpans = new HashSet<Class>(Arrays.asList(
            LeadingMarginSpan.class,
            TabStopSpan.class,
            LineHeightSpan.class,
            ReplacementSpan.class,
            MetricAffectingSpan.class
    ));

    private final SpannableStringBuilder builder;

    private int index;

    public ColoredText(String rawText) {
        assert rawText != null;
        this.builder = new SpannableStringBuilder(rawText);
    }

    public int colorWith(ColoringList colorings) {
        int processed = colorings.count();

        for (int i = 0; i < colorings.count(); i++) {
            long coloring = colorings.get(i);
            int color = getColor(coloring);
            int count = getCount(coloring);

            if (this.index + count <= this.length()) {
                this.advance(color, count);
                continue;
            }

            // This coloring extends past the end of the text.
            // Split it into 2 regions, `before` and `after`.
            // Update the coloring to only include `after` for the next ColoredText.
            int before = this.length() - this.index;
            int after = count - before;

            if (before > 0) {
                colorings.set(i, makeColoring(color, after));
                this.advance(color, before);
            }

            // Don't count the current coloring. We want the next ColoredText to see it.
            processed = i;
            break;
        }

        return processed;
    }

    private void advance(int color, int count) {
        assert count > 0;

        Object span = new ForegroundColorSpan(color);
        this.setSpan(span, this.index, this.index + count, SPAN_INCLUSIVE_EXCLUSIVE);
        this.index += count;
    }

    private static int getColor(long coloring) {
        return (int)(coloring >> 32);
    }

    private static int getCount(long coloring) {
        return (int)coloring;
    }

    private static long makeColoring(int color, int count) {
        assert count > 0; // Negative numbers are sign-extended from int -> long
        return ((long)color << 32) | count;
    }

    @Override
    public Editable replace(int st, int en, CharSequence source, int start, int end) {
        this.builder.replace(st, en, source, start, end);
        return this;
    }

    @Override
    public Editable replace(int st, int en, CharSequence text) {
        this.builder.replace(st, en, text);
        return this;
    }

    @Override
    public Editable insert(int where, CharSequence text, int start, int end) {
        this.builder.insert(where, text, start, end);
        return this;
    }

    @Override
    public Editable insert(int where, CharSequence text) {
        this.builder.insert(where, text);
        return this;
    }

    @Override
    public Editable delete(int st, int en) {
        this.builder.delete(st, en);
        return this;
    }

    @Override
    public Editable append(CharSequence text) {
        this.builder.append(text);
        return this;
    }

    @Override
    public Editable append(CharSequence text, int start, int end) {
        this.builder.append(text, start, end);
        return this;
    }

    @Override
    public Editable append(char text) {
        this.builder.append(text);
        return this;
    }

    @Override
    public void clear() {
        this.builder.clear();
    }

    @Override
    public void clearSpans() {
        this.builder.clearSpans();
    }

    @Override
    public void setFilters(InputFilter[] filters) {
        this.builder.setFilters(filters);
    }

    @Override
    public InputFilter[] getFilters() {
        return this.builder.getFilters();
    }

    @Override
    public void getChars(int start, int end, char[] dest, int destoff) {
        this.builder.getChars(start, end, dest, destoff);
    }

    @Override
    public void setSpan(Object what, int start, int end, int flags) {
        this.builder.setSpan(what, start, end, flags);
    }

    @Override
    public void removeSpan(Object what) {
        this.builder.removeSpan(what);
    }

    @Override
    public <T> T[] getSpans(int start, int end, Class<T> type) {
        return this.builder.getSpans(start, end, type);
    }

    @Override
    public int getSpanStart(Object tag) {
        return this.builder.getSpanStart(tag);
    }

    @Override
    public int getSpanEnd(Object tag) {
        return this.builder.getSpanEnd(tag);
    }

    @Override
    public int getSpanFlags(Object tag) {
        return this.builder.getSpanFlags(tag);
    }

    @Override
    public int nextSpanTransition(int start, int limit, Class type) {
        return this.builder.nextSpanTransition(start, limit, type);
    }

    @Override
    public int length() {
        return this.builder.length();
    }

    @Override
    public char charAt(int index) {
        return this.builder.charAt(index);
    }

    @Override
    public CharSequence subSequence(int start, int end) {
        return this.builder.subSequence(start, end);
    }
}
