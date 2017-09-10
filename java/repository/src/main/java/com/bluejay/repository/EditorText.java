package com.bluejay.repository;

import android.support.annotation.ColorInt;
import android.text.SpannableStringBuilder;
import android.text.style.*;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class EditorText extends SpannableStringBuilder {
    private final EditQueue mPendingEdits;

    private int mColorCursor;

    public EditorText(String rawText) {
        super(rawText);

        mPendingEdits = new EditQueue();
    }

    public void addColorings(ColoringList colorings) {
        for (int i = 0; i < colorings.count(); i++) {
            @ColorInt int color = colorings.getColor(i);
            int count = colorings.getCount(i);
            colorWith(color, count);
        }
    }

    @Override
    public SpannableStringBuilder replace(int start, int end, CharSequence tb, int tbstart, int tbend) {
        if (start != end) {
            registerDeletion(start, end - start);
        }

        if (tbstart != tbend) {
            registerInsertion(start, tbend - tbstart);
        }

        return super.replace(start, end, tb, tbstart, tbend);
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

        Edit nextEdit = mPendingEdits.peek();
        if (nextEdit != null) {
            Verify.isTrue(mColorCursor <= nextEdit.start());

            if (mColorCursor + count > nextEdit.start()) {
                int beforeCount = nextEdit.start() - mColorCursor;
                colorWith(color, beforeCount);
                count -= beforeCount;

                switch (nextEdit.type()) {
                    case DELETION:
                        if (count < nextEdit.count()) {
                            nextEdit.setCount(nextEdit.count() - count);
                            return;
                        }
                        count -= nextEdit.count();
                        break;
                    case INSERTION:
                        mColorCursor += nextEdit.count();
                        break;
                }

                mPendingEdits.remove();
                if (count != 0) {
                    colorWith(color, count);
                }
                return;
            }
        }

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

    private void registerDeletion(int start, int count) {
        Verify.isTrue(start >= 0);
        Verify.isTrue(count > 0);

        if (start < mColorCursor) {
            int beforeCount = Math.min(mColorCursor - start, count);
            mColorCursor -= beforeCount;
        }
        int end = start + count;
        if (end > mColorCursor) {
            int afterStart = Math.max(mColorCursor, start);
            int afterCount = end - afterStart;
            Verify.isTrue(afterCount == Math.min(end - mColorCursor, count));
            mPendingEdits.addDeletion(afterStart, afterCount);
        }
    }

    private void registerInsertion(int start, int count) {
        Verify.isTrue(start >= 0);
        Verify.isTrue(count > 0);

        if (start <= mColorCursor) {
            mColorCursor += count;
            return;
        }

        mPendingEdits.addInsertion(start, count);
    }

    private void removeSpans(Object[] spans) {
        for (Object span : spans) {
            removeSpan(span);
        }
    }
}
