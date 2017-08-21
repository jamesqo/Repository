package com.bluejay.repository;

import android.support.annotation.ColorInt;

import java.nio.ByteBuffer;

public class ColoringList {
    private static final int COLORING_SIZE = 8;

    private final ByteBuffer buffer;
    private final int byteStart;
    private final int count;

    private ColoringList(ByteBuffer buffer, int byteStart, int count) {
        Verify.isTrue(buffer != null);
        Verify.isTrue(byteStart >= 0);
        Verify.isTrue(count > 0);

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

    @ColorInt
    public int getColor(int index) {
        Verify.isTrue(index >= 0 && index < this.count());

        int byteIndex = this.getByteIndex(index);
        return this.buffer.getInt(byteIndex);
    }

    public int getCount(int index) {
        Verify.isTrue(index >= 0 && index < this.count());

        int byteIndex = this.getByteIndex(index);
        return this.buffer.getInt(byteIndex + 4);
    }

    private int getByteIndex(int index) {
        return this.byteStart + index * COLORING_SIZE;
    }
}
