package com.bluejay.repository;

import android.text.Editable;
import android.text.TextWatcher;

import static com.bluejay.repository.Validation.*;

public class HighlightRequester implements TextWatcher {
    private final Runnable mOnInitialRequest;
    private final int mMaxEditsBeforeRequest;

    private int mNewEdits;
    private int mPendingRequests;

    public HighlightRequester(Runnable onInitialRequest, int maxEditsBeforeRequest) {
        mOnInitialRequest = onInitialRequest;
        mMaxEditsBeforeRequest = maxEditsBeforeRequest;
        // We always want to highlight once just after the file loads, even though no edits have been made yet.
        mPendingRequests = 1;
    }

    public boolean isHighlightRequested() {
        return mPendingRequests > 0;
    }

    public void onHighlightFinished() {
        requireTrue(isHighlightRequested(), "this");

        mPendingRequests--;
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
        if (canSendRequest() && ++mNewEdits == mMaxEditsBeforeRequest) {
            sendRequest();
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
        return mPendingRequests <= 1;
    }

    private void sendRequest() {
        requireTrue(canSendRequest(), "this");

        mNewEdits = 0;

        // NOTE: 'onInitialRequest' refers to when we switch from no pending requests to 1.
        // It does not mean it fires only the very first time this method is called.
        if (++mPendingRequests == 1) {
            mOnInitialRequest.run();
        }
    }
}
