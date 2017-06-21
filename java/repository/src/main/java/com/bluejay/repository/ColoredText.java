package com.bluejay.repository;

import android.text.Editable;
import android.text.InputFilter;
import android.text.SpannableStringBuilder;
import android.text.style.ForegroundColorSpan;

import java.nio.ByteBuffer;
import java.util.IdentityHashMap;

public class ColoredText implements Editable {
    private final SpannableStringBuilder builder;
    private final IdentityHashMap<Object, UiThreadWatcher> uiThreadWatchers;

    private int index;

    public ColoredText(String rawText) {
        assert rawText != null;

        this.builder = new SpannableStringBuilder(rawText);
        this.uiThreadWatchers = new IdentityHashMap<>();
    }

    public void receive(ByteBuffer colorings, int coloringCount) {
        synchronized (this) {
            int byteCount = coloringCount * 8;
            for (int i = 0; i < byteCount; i += 8) {
                long coloring = colorings.getLong(i);
                this.advance(getColor(coloring), getCount(coloring));
            }
        }

        this.flushUiThreadWatchers();
    }

    private void advance(int color, int count) {
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
        this.clearUiThreadWatchers();
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
        this.builder.setSpan(this.wrap(o), i, i1, i2);
    }

    @Override
    public void removeSpan(Object o) {
        this.builder.removeSpan(this.findWrapper(o));
    }

    @Override
    public synchronized <T> T[] getSpans(int i, int i1, Class<T> aClass) {
        return this.builder.getSpans(i, i1, aClass);
    }

    @Override
    public int getSpanStart(Object o) {
        return this.builder.getSpanStart(this.findWrapper(o));
    }

    @Override
    public int getSpanEnd(Object o) {
        return this.builder.getSpanEnd(this.findWrapper(o));
    }

    @Override
    public int getSpanFlags(Object o) {
        return this.builder.getSpanFlags(this.findWrapper(o));
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

    private void clearUiThreadWatchers() {
        this.uiThreadWatchers.clear();
    }

    private UiThreadWatcher createWrapper(Object span) {
        UiThreadWatcher wrapper = UiThreadWatcher.create(span);
        this.uiThreadWatchers.put(span, wrapper);
        return wrapper;
    }

    private Object findWrapper(Object span) {
        return UiThreadWatcher.canCreate(span) ? this.uiThreadWatchers.get(span) : span;
    }

    private void flushUiThreadWatchers() {
        for (UiThreadWatcher watcher : this.uiThreadWatchers.values()) {
            watcher.flush();
        }
    }

    private Object wrap(Object span) {
        Object wrapper = this.findWrapper(span);
        return wrapper != null ? wrapper : this.createWrapper(span);
    }
}
