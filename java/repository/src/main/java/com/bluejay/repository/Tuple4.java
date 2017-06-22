package com.bluejay.repository;

public class Tuple4<T1, T2, T3, T4> {
    public final T1 item1;
    public final T2 item2;
    public final T3 item3;
    public final T4 item4;

    public Tuple4(T1 item1, T2 item2, T3 item3, T4 item4) {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
        this.item4 = item4;
    }
}
