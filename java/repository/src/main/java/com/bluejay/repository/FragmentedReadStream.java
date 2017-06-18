package com.bluejay.repository;

import java.io.EOFException;
import java.nio.ByteBuffer;

public class FragmentedReadStream {
    private final ByteBuffer[] fragments;
    private final int byteCount;

    private int bufferIndex;
    private int byteIndex;
    private int bytesRead;

    public FragmentedReadStream(ByteBuffer[] fragments, int byteCount) {
        assert fragments != null;
        assert byteCount >= 0;

        this.fragments = fragments.clone();
        this.byteCount = byteCount;
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

    private ByteBuffer getCurrentFragment() {
        assert this.hasMore();

        ByteBuffer current = this.fragments[this.bufferIndex];
        if (this.byteIndex == current.capacity()) {
            moveToNextFragment();
            current = this.fragments[this.bufferIndex];
        }
        return current;
    }

    private void moveToNextFragment() {
        this.bufferIndex++;
        this.byteIndex = 0;
    }
}
