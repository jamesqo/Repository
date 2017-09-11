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
            addColoring(color, count);
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

    private void addColoring(@ColorInt int color, int count) {
        Verify.isTrue(mColorCursor >= 0);
        Verify.isTrue(count > 0);
        Verify.isTrue(mColorCursor + count <= length());

        Edit edit = mPendingEdits.peek();
        if (edit != null) {
            // The color cursor shouldn't have reached the edit yet.
            Verify.isTrue(mColorCursor <= edit.start());

            // Will we overlap with the edit during this coloring?
            if (mColorCursor + count > edit.start()) {
                addColoringAndHandleEdit(color, count);
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

    private void addColoringAndHandleEdit(@ColorInt int color, int count) {
        Verify.isTrue(count > 0);

        Edit edit = mPendingEdits.element();

        // Color the text before the start of the edit.
        int beforeCount = edit.start() - mColorCursor;
        addColoring(color, beforeCount);
        count -= beforeCount;

        if (edit.isInsertion()) {
            // We haven't parsed the inserted text so we can't assume anything about how it should be colored.
            // Skip it and leave it white.
            mColorCursor += edit.count();
        } else {
            // Deletion
            // Throw away the part of the coloring that is inside the deleted region.
            if (count < edit.count()) {
                // The coloring ends inside the deleted region.
                edit.setCount(edit.count() - count);
                return;
            }
            // The coloring ends at or after the end of the deleted region.
            count -= edit.count();
        }

        mPendingEdits.remove();

        // addColoring() expects the count to be greater than 0.
        // The count can be 0 if the end of the coloring coincides with the end of a deleted region.
        if (count != 0) {
            addColoring(color, count);
        }
    }

    private void advanceColorCursor(int count) {
        mColorCursor += count;
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
