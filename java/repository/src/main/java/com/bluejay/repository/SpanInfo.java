package com.bluejay.repository;

public class SpanInfo {
    public final Object span;
    public final int start;
    public final int end;
    public final int flags;

    public SpanInfo(Object span, int start, int end, int flags) {
        this.span = span;
        this.start = start;
        this.end = end;
        this.flags = flags;
    }
}
