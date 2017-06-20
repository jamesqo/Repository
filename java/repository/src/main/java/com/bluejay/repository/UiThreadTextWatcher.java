package com.bluejay.repository;

import android.text.Editable;
import android.text.TextWatcher;

public class UiThreadTextWatcher implements TextWatcher, UiThreadWatcher {
    private final TextWatcher watcher;

    public UiThreadTextWatcher(TextWatcher watcher) {
        this.watcher = watcher;
    }

    @Override
    public void beforeTextChanged(final CharSequence charSequence, final int i, final int i1, final int i2) {
        final TextWatcher watcher = this.watcher;
        Threading.postToUiThread(new Runnable() {
            @Override
            public void run() {
                watcher.beforeTextChanged(charSequence, i, i1, i2);
            }
        });
    }

    @Override
    public void onTextChanged(final CharSequence charSequence, final int i, final int i1, final int i2) {
        final TextWatcher watcher = this.watcher;
        Threading.postToUiThread(new Runnable() {
            @Override
            public void run() {
                watcher.onTextChanged(charSequence, i, i1, i2);
            }
        });
    }

    @Override
    public void afterTextChanged(final Editable editable) {
        final TextWatcher watcher = this.watcher;
        Threading.postToUiThread(new Runnable() {
            @Override
            public void run() {
                watcher.afterTextChanged(editable);
            }
        });
    }
}
