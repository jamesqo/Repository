package com.bluejay.repository;

import java.nio.ByteBuffer;

public class ColoringList {
    private final ByteBuffer buffer;
    private final int byteStart;
    private final int count;

    private ColoringList(ByteBuffer buffer, int byteStart, int count) {
        assert buffer != null;
        assert byteStart >= 0;
        assert count > 0;

        this.buffer = buffer;
        this.byteStart = byteStart;
        this.count = count;
    }

    public static ColoringList fromBufferSpan(ByteBuffer buffer, int byteStart, int count) {
        return new ColoringList(buffer, byteStart, count);
    }

    public int count() {
        return this.count;
    }

    public long get(int index) {
        int byteIndex = this.getByteIndex(index);
        return this.buffer.getLong(byteIndex);
    }

    public void set(int index, long element) {
        int byteIndex = this.getByteIndex(index);
        this.buffer.putLong(byteIndex, element);
    }

    public ColoringList slice(int index) {
        assert index < this.count();

        return new ColoringList(this.buffer, this.getByteIndex(index), this.count() - index);
    }

    private int getByteIndex(int index) {
        return this.byteStart + index * 8;
    }
}
