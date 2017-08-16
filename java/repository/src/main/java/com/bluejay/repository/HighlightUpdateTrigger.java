package com.bluejay.repository;

import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;

public class HighlightUpdateTrigger implements TextWatcher {
    private  final Runnable doHighlightUpdate;
    private final Handler handler;
    private final int maxEditsBeforeTrigger;

    private int editsSinceLastTrigger;

    public HighlightUpdateTrigger(Runnable doHighlightUpdate, Handler handler, int maxEditsBeforeTrigger) {
        this.doHighlightUpdate = doHighlightUpdate;
        this.handler = handler;
        this.maxEditsBeforeTrigger = maxEditsBeforeTrigger;
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
        // TODO: Trigger after newline or after n milliseconds.
        if (++this.editsSinceLastTrigger == this.maxEditsBeforeTrigger) {
            this.editsSinceLastTrigger = 0;
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
