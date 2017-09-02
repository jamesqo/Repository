package com.bluejay.repository;

import android.support.annotation.ColorInt;
import android.text.SpannableStringBuilder;
import android.text.style.*;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class EditorText extends SpannableStringBuilder {
    private int mColorCursor;

    public EditorText(String rawText) {
        super(rawText);
    }

    public void addColorings(ColoringList colorings) {
        for (int i = 0; i < colorings.count(); i++) {
            @ColorInt int color = colorings.getColor(i);
            int count = colorings.getCount(i);
            colorWith(color, count);
        }
    }

    public void resetColorCursor() {
        mColorCursor = 0;
    }

    private void advanceColorCursor(int count) {
        mColorCursor += count;
    }

    private void colorWith(@ColorInt int color, int count) {
        Verify.isTrue(mColorCursor >= 0);
        Verify.isTrue(count > 0);
        Verify.isTrue(mColorCursor + count <= length());

        int start = mColorCursor, end = start + count;
        ForegroundColorSpan[] overlaps = getSpans(start, end, ForegroundColorSpan.class);

        if (overlaps.length > 0) {
            if (overlaps.length == 1 && overlaps[0].getForegroundColor() == color) {
                // 'start' and 'end' are in the same span and already have this color.
                advanceColorCursor(count);
                return;
            }

            makeGap(start, end, overlaps);
        }

        ForegroundColorSpan span = new ForegroundColorSpan(color);
        setSpan(span, start, end, SPAN_INCLUSIVE_EXCLUSIVE);
        advanceColorCursor(count);
    }

    private void makeGap(int start, int end, ForegroundColorSpan[] overlaps) {
        ForegroundColorSpan first = overlaps[0];
        int firstStart = getSpanStart(first);

        ForegroundColorSpan last = overlaps[overlaps.length - 1];
        int lastEnd = getSpanEnd(last);

        removeSpans(overlaps);

        if (firstStart < start) {
            setSpan(first, firstStart, start, SPAN_INCLUSIVE_EXCLUSIVE);
        }

        if (lastEnd > end) {
            setSpan(last, end, lastEnd, SPAN_INCLUSIVE_EXCLUSIVE);
        }
    }

    private void removeSpans(Object[] spans) {
        for (Object span : spans) {
            removeSpan(span);
        }
    }
}
