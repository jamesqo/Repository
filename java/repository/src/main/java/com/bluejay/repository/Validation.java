package com.bluejay.repository;

class Validation {
    private Validation() {
    }

    public static <T> void requireNonEmpty(T[] argument, String argumentName) {
        requireNonNull(argument, argumentName);

        if (argument.length == 0) {
            throw new IllegalArgumentException(String.format("%s was empty", argumentName));
        }
    }

    public static void requireNonNull(Object argument, String argumentName) {
        if (argument == null) {
            throw new IllegalArgumentException(String.format("%s was null", argumentName));
        }
    }

    public static void requireRange(boolean condition, String argumentName) {
        if (!condition) {
            throw new IllegalArgumentException(String.format("%s was out of range", argumentName));
        }
    }

    public static void requireTrue(boolean condition, String argumentName) {
        if (!condition) {
            throw new IllegalArgumentException(String.format("The condition involving %s was false", argumentName));
        }
    }
}
