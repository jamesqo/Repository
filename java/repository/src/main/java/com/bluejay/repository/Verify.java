package com.bluejay.repository;

class Verify {
    private Verify() {
    }

    public static void isTrue(boolean condition) {
        if (BuildConfig.DEBUG && !condition) {
            throw new AssertionError();
        }
    }
}
