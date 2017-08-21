package com.bluejay.repository;

import android.support.annotation.ColorInt;
import android.text.SpannableStringBuilder;
import android.text.style.*;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class EditorText extends SpannableStringBuilder {
    private int colorCursor;

    public EditorText(String rawText) {
        super(rawText);
    }

    public void colorWith(ColoringList colorings) {
        for (int i = 0; i < colorings.count(); i++) {
            @ColorInt int color = colorings.getColor(i);
            int count = colorings.getCount(i);
            this.colorWith(color, count);
        }
    }

    public void resetColorCursor() {
        this.colorCursor = 0;
    }

    private void advanceColorCursor(int count) {
        this.colorCursor += count;
    }

    private void colorWith(@ColorInt int color, int count) {
        Verify.isTrue(this.colorCursor >= 0);
        Verify.isTrue(count > 0);
        Verify.isTrue(this.colorCursor + count <= this.length());

        int start = this.colorCursor, end = start + count;
        ForegroundColorSpan[] overlaps = this.getSpans(start, end, ForegroundColorSpan.class);

        if (overlaps.length > 0) {
            if (overlaps.length == 1 && overlaps[0].getForegroundColor() == color) {
                // 'start' and 'end' are in the same span and already have this color.
                this.advanceColorCursor(count);
                return;
            }

            this.makeGap(start, end, overlaps);
        }

        ForegroundColorSpan span = new ForegroundColorSpan(color);
        this.setSpan(span, start, end, SPAN_INCLUSIVE_EXCLUSIVE);
        this.advanceColorCursor(count);
    }

    private void makeGap(int start, int end, ForegroundColorSpan[] overlaps) {
        ForegroundColorSpan first = overlaps[0];
        int firstStart = this.getSpanStart(first);

        ForegroundColorSpan last = overlaps[overlaps.length - 1];
        int lastEnd = this.getSpanEnd(last);

        this.removeSpans(overlaps);

        if (firstStart < start) {
            this.setSpan(first, firstStart, start, SPAN_INCLUSIVE_EXCLUSIVE);
        }

        if (lastEnd > end) {
            this.setSpan(last, end, lastEnd, SPAN_INCLUSIVE_EXCLUSIVE);
        }
    }

    private void removeSpans(Object[] spans) {
        for (Object span : spans) {
            this.removeSpan(span);
        }
    }
}
