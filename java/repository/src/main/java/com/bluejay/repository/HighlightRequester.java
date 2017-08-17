package com.bluejay.repository;

import android.text.Editable;
import android.text.TextWatcher;

public class HighlightRequester implements TextWatcher {
    public interface OnInitialRequestCallback {
        void run(HighlightRequester requester);
    }

    private final OnInitialRequestCallback onInitialRequest;
    private final int maxEditsBeforeRequest;

    private int newEdits;
    private int pendingRequests;

    public HighlightRequester(OnInitialRequestCallback onInitialRequest, int maxEditsBeforeRequest) {
        this.onInitialRequest = onInitialRequest;
        this.maxEditsBeforeRequest = maxEditsBeforeRequest;
        this.pendingRequests = 1;
    }

    public boolean isHighlightRequested() {
        return this.pendingRequests > 0;
    }

    public void onHighlightFinished() {
        assert this.isHighlightRequested();
        this.pendingRequests--;
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
        // TODO: Update after newline or after n milliseconds.
        if (this.canSendRequest() && ++this.newEdits == this.maxEditsBeforeRequest) {
            this.sendRequest();
        }
    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {
    }

    @Override
    public void afterTextChanged(Editable s) {
    }

    private boolean canSendRequest() {
        return this.pendingRequests <= 1;
    }

    private void sendRequest() {
        assert this.canSendRequest();

        this.newEdits = 0;

        if (++this.pendingRequests == 1) {
            this.onInitialRequest.run(this);
        }
    }
}
