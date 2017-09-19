package com.bluejay.repository;

import android.support.annotation.ColorInt;

import java.nio.ByteBuffer;

import static com.bluejay.repository.Validation.*;

public class ColoringList {
    private static final int COLORING_SIZE = 8;

    private final ByteBuffer mBuffer;
    private final int mByteStart;
    private final int mCount;

    private ColoringList(ByteBuffer buffer, int byteStart, int count) {
        requireNonNull(buffer, "buffer");
        requireRange(byteStart >= 0, "byteStart");
        requireRange(count > 0, "count");

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
        requireRange(index >= 0 && index < count(), "index");

        int byteIndex = getByteIndex(index);
        return mBuffer.getInt(byteIndex);
    }

    public int getCount(int index) {
        requireRange(index >= 0 && index < count(), "index");

        int byteIndex = getByteIndex(index);
        return mBuffer.getInt(byteIndex + 4);
    }

    private int getByteIndex(int index) {
        return mByteStart + index * COLORING_SIZE;
    }
}
