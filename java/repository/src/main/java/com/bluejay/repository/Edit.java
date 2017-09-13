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

    public boolean containsInclusive(int index) {
        Verify.isTrue(index >= 0);

        return index >= start() && index <= end();
    }

    public int count() {
        return mCount;
    }

    public int end() {
        return start() + count();
    }

    @Override
    public boolean equals(Object obj) {
        return obj instanceof Edit && equals((Edit) obj);
    }

    public boolean equals(Edit other) {
        return other != null &&
                start() == other.start() &&
                count() == other.count();
    }

    public boolean isInsertion() {
        return mIsInsertion;
    }

    public int start() {
        return mStart;
    }

    public void setCount(int count) {
        Verify.isTrue(count > 0);

        mCount = count;
    }

    public void setStart(int start) {
        Verify.isTrue(start >= 0);

        mStart = start;
    }

    @Override
    public String toString() {
        String prefix = isInsertion() ? "I" : "D";
        return prefix + "@[" + start() + ".." + end() + ")";
    }
}
