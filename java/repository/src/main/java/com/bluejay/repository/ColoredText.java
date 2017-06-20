package com.bluejay.repository;

import android.text.Editable;
import android.text.InputFilter;
import android.text.Spannable;
import android.text.SpannableStringBuilder;
import android.text.Spanned;
import android.text.style.ForegroundColorSpan;

import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class ColoredText implements Editable {
    private final SpannableStringBuilder builder;

    public ColoredText(String rawText, FragmentedReadStream colorings) {
        assert rawText != null;
        assert colorings != null;

        this.builder = new SpannableStringBuilder(rawText);
        this.colorWith(colorings);
    }

    private void colorWith(FragmentedReadStream colorings) {
        int index = 0;
        while (colorings.hasMore()) {
            long coloring = colorings.readLong();
            int color = getColor(coloring);
            int count = getCount(coloring);

            Object span = new ForegroundColorSpan(color);
            this.setSpan(span, index, index + count, SPAN_INCLUSIVE_EXCLUSIVE);
            index += count;
        }

        assert index == this.length();
    }

    private static int getColor(long coloring) {
        return (int)(coloring >> 32);
    }

    private static int getCount(long coloring) {
        return (int)coloring;
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
