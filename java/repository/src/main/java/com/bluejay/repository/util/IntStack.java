package com.bluejay.repository.util;

public final class IntStack {
    private final int[] mArray;
    private int mSize;

    public IntStack(int capacity) {
        mArray = new int[capacity];
    }

    public boolean isEmpty() {
        return mSize == 0;
    }

    public int peek() {
        return mArray[mSize - 1];
    }

    public int pop() {
        return mArray[--mSize];
    }

    public void push(int item) {
        mArray[mSize++] = item;
    }
}
