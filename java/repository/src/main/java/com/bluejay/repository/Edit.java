package com.bluejay.repository;

class Edit {
    private final EditType mType;

    private int mStart;
    private int mCount;

    public Edit(EditType type, int start, int count) {
        mType = type;
        setStart(start);
        setCount(count);
    }

    public int count() {
        return mCount;
    }

    public void setCount(int count) {
        Verify.isTrue(count > 0);

        mCount = count;
    }

    public int start() {
        return mStart;
    }

    public void setStart(int start) {
        Verify.isTrue(start >= 0);

        mStart = start;
    }

    public EditType type() {
        return mType;
    }
}
