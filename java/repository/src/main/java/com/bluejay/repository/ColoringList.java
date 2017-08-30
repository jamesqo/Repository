package com.bluejay.repository;

import android.support.annotation.ColorInt;

import java.nio.ByteBuffer;

public class ColoringList {
    private static final int COLORING_SIZE = 8;

    private final ByteBuffer mBuffer;
    private final int mByteStart;
    private final int mCount;

    private ColoringList(ByteBuffer buffer, int byteStart, int count) {
        Verify.isTrue(buffer != null);
        Verify.isTrue(byteStart >= 0);
        Verify.isTrue(count > 0);

        mBuffer = buffer;
        mByteStart = byteStart;
        mCount = count;
    }

    public static ColoringList fromBufferSpan(ByteBuffer buffer, int byteStart, int count) {
        return new ColoringList(buffer, byteStart, count);
    }

    public int count() {
        return mCount;
    }

    @ColorInt
    public int getColor(int index) {
        Verify.isTrue(index >= 0 && index < count());

        int byteIndex = getByteIndex(index);
        return mBuffer.getInt(byteIndex);
    }

    public int getCount(int index) {
        Verify.isTrue(index >= 0 && index < count());

        int byteIndex = getByteIndex(index);
        return mBuffer.getInt(byteIndex + 4);
    }

    private int getByteIndex(int index) {
        return mByteStart + index * COLORING_SIZE;
    }
}
