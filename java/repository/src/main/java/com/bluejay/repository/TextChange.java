package com.bluejay.repository;

public class TextChange {
    private final int start;
    private final int diff;

    public TextChange(int start, int diff) {
        assert start >= 0;
        assert diff != 0;

        this.start = start;
        this.diff = diff;
    }

    public int diff() {
        return this.diff;
    }

    public int start() {
        return this.start;
    }
}
