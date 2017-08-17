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
        // We always want to highlight once just after the file loads, even though no edits have been made yet.
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
        // There is no point in having 3+ requests pending at once.
        // Say a request fires every 10 chars. The user types 100 chars,
        // 10 before the first request and 90 after the first and before
        // the second. (S)he then stops typing.
        // After the first highlight finishes, it makes sense to run a *single*
        // second highlight to account for the 90 new chars. However, it's clearly
        // pointless to run the highlighter 9 times against the same exact text.
        return this.pendingRequests <= 1;
    }

    private void sendRequest() {
        assert this.canSendRequest();

        this.newEdits = 0;

        // NOTE: 'onInitialRequest' refers to when we switch from no pending requests to 1.
        // It does not mean it fires only the very first time this method is called.
        if (++this.pendingRequests == 1) {
            this.onInitialRequest.run(this);
        }
    }
}
