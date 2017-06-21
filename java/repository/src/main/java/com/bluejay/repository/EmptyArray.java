package com.bluejay.repository;

import java.lang.reflect.Array;

public final class EmptyArray {
    private static final int CACHE_SIZE = 64;
    private static final Object[] cache = new Object[CACHE_SIZE];

    private EmptyArray() {
        assert false;
    }

    // Implementation mostly stolen from Android's ArrayUtils class.
    public static <T> T[] get(Class<T> kind) {
        int bucket = (kind.hashCode() & Integer.MAX_VALUE) % CACHE_SIZE;
        Object array = cache[bucket];

        if (array == null || array.getClass().getComponentType() != kind) {
            array = cache[bucket] = Array.newInstance(kind, 0);
        }

        return (T[]) array;
    }
}
