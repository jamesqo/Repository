package com.bluejay.repository;

import android.text.SpanWatcher;
import android.text.Spannable;

public class UiThreadSpanWatcher implements SpanWatcher, UiThreadWatcher {
    private final SpanWatcher watcher;

    public UiThreadSpanWatcher(SpanWatcher watcher) {
        this.watcher = watcher;
    }

    @Override
    public void onSpanAdded(final Spannable spannable, final Object o, final int i, final int i1) {
        final SpanWatcher watcher = this.watcher;
        Threading.postToUiThread(new Runnable() {
            @Override
            public void run() {
                watcher.onSpanAdded(spannable, o, i, i1);
            }
        });
    }

    @Override
    public void onSpanRemoved(final Spannable spannable, final Object o, final int i, final int i1) {
        final SpanWatcher watcher = this.watcher;
        Threading.postToUiThread(new Runnable() {
            @Override
            public void run() {
                watcher.onSpanRemoved(spannable, o, i, i1);
            }
        });
    }

    @Override
    public void onSpanChanged(final Spannable spannable, final Object o, final int i, final int i1, final int i2, final int i3) {
        final SpanWatcher watcher = this.watcher;
        Threading.postToUiThread(new Runnable() {
            @Override
            public void run() {
                watcher.onSpanChanged(spannable, o, i, i1, i2, i3);
            }
        });
    }
}
