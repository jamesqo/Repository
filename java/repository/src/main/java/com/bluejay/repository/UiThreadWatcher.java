package com.bluejay.repository;

import android.text.Editable;
import android.text.SpanWatcher;
import android.text.Spannable;
import android.text.TextWatcher;

public class UiThreadWatcher implements SpanWatcher, TextWatcher {
    private final SpanWatcher spanWatcher;
    private final TextWatcher textWatcher;

    private UiThreadWatcher(Object span) {
        this.spanWatcher = span instanceof SpanWatcher ? (SpanWatcher) span : null;
        this.textWatcher = span instanceof TextWatcher ? (TextWatcher) span : null;
    }

    public static boolean canCreate(Object span) {
        return span instanceof SpanWatcher || span instanceof TextWatcher;
    }

    public static UiThreadWatcher create(Object span) {
        return canCreate(span) ? new UiThreadWatcher(span) : null;
    }

    @Override
    public void onSpanAdded(final Spannable spannable, final Object o, final int i, final int i1) {
        final SpanWatcher watcher = this.spanWatcher;
        if (watcher != null) {
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    watcher.onSpanAdded(spannable, o, i, i1);
                }
            });
        }
    }

    @Override
    public void onSpanRemoved(final Spannable spannable, final Object o, final int i, final int i1) {
        final SpanWatcher watcher = this.spanWatcher;
        if (watcher != null) {
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    watcher.onSpanRemoved(spannable, o, i, i1);
                }
            });
        }
    }

    @Override
    public void onSpanChanged(final Spannable spannable, final Object o, final int i, final int i1, final int i2, final int i3) {
        final SpanWatcher watcher = this.spanWatcher;
        if (watcher != null) {
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    watcher.onSpanChanged(spannable, o, i, i1, i2, i3);
                }
            });
        }
    }

    @Override
    public void beforeTextChanged(final CharSequence charSequence, final int i, final int i1, final int i2) {
        final TextWatcher watcher = this.textWatcher;
        if (watcher != null) {
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    watcher.beforeTextChanged(charSequence, i, i1, i2);
                }
            });
        }
    }

    @Override
    public void onTextChanged(final CharSequence charSequence, final int i, final int i1, final int i2) {
        final TextWatcher watcher = this.textWatcher;
        if (watcher != null) {
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    watcher.onTextChanged(charSequence, i, i1, i2);
                }
            });
        }
    }

    @Override
    public void afterTextChanged(final Editable editable) {
        final TextWatcher watcher = this.textWatcher;
        if (watcher != null) {
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    watcher.afterTextChanged(editable);
                }
            });
        }
    }
}
