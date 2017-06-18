package com.bluejay.repository;

import java.io.EOFException;
import java.nio.ByteBuffer;

public class FragmentedByteStream {
    private final ByteBuffer[] fragments;
    private final int byteCount;

    private int bufferIndex;
    private int byteIndex;
    private int bytesRead;

    public FragmentedByteStream(ByteBuffer[] fragments) {
        assert fragments != null;

        this.fragments = fragments.clone();
        this.byteCount = getByteCount(fragments);
    }

    public int byteCount() {
        return this.byteCount;
    }

    public boolean hasMore() {
        return this.bytesRead != this.byteCount;
    }

    public long readLong() {
        ByteBuffer current = this.getCurrentFragment();
        long result = current.getLong(this.byteIndex);
        this.advance(8);
        return result;
    }

    private void advance(int byteCount) {
        this.byteIndex += byteCount;
        this.bytesRead += byteCount;
    }

    private static int getByteCount(ByteBuffer[] fragments) {
        int count = 0;
        for (int i = 0; i < fragments.length; i++) {
            count += fragments[i].capacity();
        }
        return count;
    }

    private ByteBuffer getCurrentFragment() {
        ByteBuffer current = this.fragments[this.bufferIndex];
        if (this.byteIndex == current.capacity()) {
            if (!moveToNextFragment()) {
                throw new IllegalStateException("No more bytes available!");
            }
            current = this.fragments[this.bufferIndex];
        }
        return current;
    }

    private boolean moveToNextFragment() {
        this.bufferIndex++;
        this.byteIndex = 0;
        return this.bufferIndex != this.fragments.length;
    }
}
