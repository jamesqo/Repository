package com.bluejay.repository;

import android.os.Handler;
import android.os.Looper;

public final class Threading {
    private static final Handler uiThreadHandler = new Handler(Looper.getMainLooper());

    private Threading() {
        assert false;
    }

    public static boolean postToUiThread(Runnable r) {
        return uiThreadHandler.post(r);
    }
}
