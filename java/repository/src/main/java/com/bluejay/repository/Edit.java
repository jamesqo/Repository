package com.bluejay.repository;

import static com.bluejay.repository.Validation.*;

public class Edit {
    public enum Bounds {
        INCLUSIVE_EXCLUSIVE,
        INCLUSIVE_INCLUSIVE
    }

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

    public boolean contains(int index, Bounds bounds) {
        requireRange(index >= 0, "index");
        requireNonNull(bounds, "bounds");

        switch (bounds) {
            case INCLUSIVE_EXCLUSIVE:
                return index >= start() && index < end();
            case INCLUSIVE_INCLUSIVE:
                return index >= start() && index <= end();
            default:
                throw new UnsupportedOperationException();
        }
    }

    public int count() {
        return mCount;
    }

    public int diff() {
        return isInsertion() ? count() : -count();
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
                isInsertion() == other.isInsertion() &&
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
        requireRange(count > 0, "count");

        mCount = count;
    }

    public void setStart(int start) {
        requireRange(start >= 0, "start");

        mStart = start;
    }

    public int visualEnd() {
        return isInsertion() ? end() : start();
    }

    @Override
    public String toString() {
        String prefix = isInsertion() ? "I" : "D";
        return prefix + "@[" + start() + ".." + end() + ")";
    }
}
