package com.bluejay.repository;

public class Edit {
    public final EditType type;

    public int start;
    public int count;

    public Edit(EditType type, int start, int count) {
        this.type = type;
        this.start = start;
        this.count = count;
    }
}
