package com.bluejay.repository;

public class Edit {
    private final boolean mIsInsertion;

    private int mStart;
    private int mCount;

    private Edit(boolean isInsertion, int start, int count) {
        mIsInsertion = isInsertion;
        setStart(start);
        setCount(count);
    }

    public static Edit deletion(int start, int count) {
        return new Edit(false, start, count);
    }

    public static Edit insertion(int start, int count) {
        return new Edit(true, start, count);
    }

    public boolean isInsertion() {
        return mIsInsertion;
    }

    public int start() {
        return mStart;
    }

    public void setStart(int start) {
        Verify.isTrue(start >= 0);

        mStart = start;
    }

    public int count() {
        return mCount;
    }

    public void setCount(int count) {
        Verify.isTrue(count > 0);

        mCount = count;
    }
}
