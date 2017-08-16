package com.bluejay.repository;

import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;

// TODO: HighlightingUpdate?
public class UpdateHighlightingTriggerer implements TextWatcher {
    private  final Runnable updateHighlighting;
    private final Handler handler;
    private final int maxCharsBeforeTrigger;

    private int charsSinceLastTrigger;

    public UpdateHighlightingTriggerer(Runnable updateHighlighting, Handler handler, int maxCharsBeforeTrigger) {
        this.updateHighlighting = updateHighlighting;
        this.handler = handler;
        this.maxCharsBeforeTrigger = maxCharsBeforeTrigger;
    }


    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
        // TODO: Trigger after newline or after n milliseconds.
        if (++this.charsSinceLastTrigger == this.maxCharsBeforeTrigger) {
            this.charsSinceLastTrigger = 0;
            this.handler.post(this.updateHighlighting);
        }
    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {

    }
}
