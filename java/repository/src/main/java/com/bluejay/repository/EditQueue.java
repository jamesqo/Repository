package com.bluejay.repository;

public class EditQueue {

    public void addDeletion(int start, int count) {
        Verify.isTrue(start >= 0);
        Verify.isTrue(count > 0);
    }

    public void addInsertion(int start, int count) {
        Verify.isTrue(start >= 0);
        Verify.isTrue(count > 0);


    }

    public Edit peek() {

    }

    public Edit remove() {

    }
}
