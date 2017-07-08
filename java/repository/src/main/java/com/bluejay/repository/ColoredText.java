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
    public Editable replace(int i, int i1, CharSequence charSequence, int i2, int i3) {
        this.builder.replace(i, i1, charSequence, i2, i3);
        return this;
    }

    @Override
    public Editable replace(int i, int i1, CharSequence charSequence) {
        this.builder.replace(i, i1, charSequence);
        return this;
    }

    @Override
    public Editable insert(int i, CharSequence charSequence, int i1, int i2) {
        this.builder.insert(i, charSequence, i1, i2);
        return this;
    }

    @Override
    public Editable insert(int i, CharSequence charSequence) {
        this.builder.insert(i, charSequence);
        return this;
    }

    @Override
    public Editable delete(int i, int i1) {
        this.builder.delete(i, i1);
        return this;
    }

    @Override
    public Editable append(CharSequence charSequence) {
        this.builder.append(charSequence);
        return this;
    }

    @Override
    public Editable append(CharSequence charSequence, int i, int i1) {
        this.builder.append(charSequence, i, i1);
        return this;
    }

    @Override
    public Editable append(char c) {
        this.builder.append(c);
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
    public void setFilters(InputFilter[] inputFilters) {
        this.builder.setFilters(inputFilters);
    }

    @Override
    public InputFilter[] getFilters() {
        return this.builder.getFilters();
    }

    @Override
    public void getChars(int i, int i1, char[] chars, int i2) {
        this.builder.getChars(i, i1, chars, i2);
    }

    @Override
    public void setSpan(Object o, int i, int i1, int i2) {
        this.builder.setSpan(o, i, i1, i2);
    }

    @Override
    public void removeSpan(Object o) {
        this.builder.removeSpan(o);
    }

    @Override
    public <T> T[] getSpans(int i, int i1, Class<T> aClass) {
        if (noSpans.contains(aClass)) {
            return EmptyArray.get(aClass);
        }
        return this.builder.getSpans(i, i1, aClass);
    }

    @Override
    public int getSpanStart(Object o) {
        return this.builder.getSpanStart(o);
    }

    @Override
    public int getSpanEnd(Object o) {
        return this.builder.getSpanEnd(o);
    }

    @Override
    public int getSpanFlags(Object o) {
        return this.builder.getSpanFlags(o);
    }

    @Override
    public int nextSpanTransition(int i, int i1, Class aClass) {
        if (noSpans.contains(aClass)) {
            return i1;
        }
        return this.builder.nextSpanTransition(i, i1, aClass);
    }

    @Override
    public int length() {
        return this.builder.length();
    }

    @Override
    public char charAt(int i) {
        return this.builder.charAt(i);
    }

    @Override
    public CharSequence subSequence(int i, int i1) {
        return this.builder.subSequence(i, i1);
    }
}
