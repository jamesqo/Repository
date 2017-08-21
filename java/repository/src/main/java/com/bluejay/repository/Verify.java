package com.bluejay.repository;

class Verify {
    private Verify() {
    }

    public static void isTrue(boolean condition) {
        if (!condition) {
            throw new AssertionError();
        }
    }
}
