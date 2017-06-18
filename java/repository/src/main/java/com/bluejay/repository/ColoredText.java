package com.bluejay.repository;

import android.text.Spannable;

public class ColoredText implements Spannable {
    private final String rawText;
    private final List<Coloring> colorings;

    public ColoredText(String rawText, List<Coloring> colorings) {

    }

    @Override
    public void setSpan(Object o, int i, int i1, int i2) {

    }

    @Override
    public void removeSpan(Object o) {

    }

    @Override
    public <T> T[] getSpans(int i, int i1, Class<T> aClass) {
        return null;
    }

    @Override
    public int getSpanStart(Object o) {
        return 0;
    }

    @Override
    public int getSpanEnd(Object o) {
        return 0;
    }

    @Override
    public int getSpanFlags(Object o) {
        return 0;
    }

    @Override
    public int nextSpanTransition(int i, int i1, Class aClass) {
        return 0;
    }

    @Override
    public int length() {
        return 0;
    }

    @Override
    public char charAt(int i) {
        return 0;
    }

    @Override
    public CharSequence subSequence(int i, int i1) {
        return null;
    }
}
