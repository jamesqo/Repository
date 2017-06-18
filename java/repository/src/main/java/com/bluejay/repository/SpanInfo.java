package com.bluejay.repository;

public class SpanInfo {
    private final int start;
    private final int end;

    public SpanInfo(int start, int end) {
        this.start = start;
        this.end = end;
    }

    public int getEnd() {
        return this.end;
    }

    public int getStart() {
        return this.start;
    }
}
