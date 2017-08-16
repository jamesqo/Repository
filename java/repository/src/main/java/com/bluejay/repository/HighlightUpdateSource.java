package com.bluejay.repository;

import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;

public class HighlightUpdateSource implements TextWatcher {
    private  final Runnable doHighlightUpdate;
    private final Handler handler;
    private final int maxEditsBeforeUpdate;

    private int editsSinceLastUpdate;

    public HighlightUpdateSource(Runnable doHighlightUpdate, Handler handler, int maxEditsBeforeUpdate) {
        this.doHighlightUpdate = doHighlightUpdate;
        this.handler = handler;
        this.maxEditsBeforeUpdate = maxEditsBeforeUpdate;
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
        // TODO: Update after newline or after n milliseconds.
        if (++this.editsSinceLastUpdate == this.maxEditsBeforeUpdate) {
            this.editsSinceLastUpdate = 0;
            this.handler.post(this.doHighlightUpdate);
        }
    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {

    }
}
