package com.bluejay.repository;

import android.support.annotation.ColorInt;
import android.text.SpannableStringBuilder;
import android.text.style.*;

import static com.bluejay.repository.Validation.*;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class EditorText extends SpannableStringBuilder {
    private static final int DORMANT_COLOR_CURSOR = -1;

    private final EditQueue mPendingEdits;

    private int mColorCursor;

    public EditorText(String rawText) {
        super(rawText);

        mPendingEdits = new EditQueue();
        resetColorCursor();
    }

    public void addColorings(ColoringList colorings) {
        requireNonNull(colorings, "colorings");

        if (isColorCursorDormant()) {
            mColorCursor = 0;
        }

        for (int i = 0; i < colorings.count(); i++) {
            @ColorInt int color = colorings.getColor(i);
            int count = colorings.getCount(i);
            addColoring(color, count);
        }
    }

    @Override
    public SpannableStringBuilder replace(int start, int end, CharSequence tb, int tbstart, int tbend) {
        if (!isColorCursorDormant()) {
            if (start != end) {
                registerDeletion(start, end - start);
            }

            if (tbstart != tbend) {
                registerInsertion(start, tbend - tbstart);
            }
        }

        return super.replace(start, end, tb, tbstart, tbend);
    }

    public void resetColorCursor() {
        mColorCursor = DORMANT_COLOR_CURSOR;
    }

    private void addColoring(@ColorInt int color, int count) {
        requireTrue(!isColorCursorDormant(), "this");
        requireRange(mColorCursor >= 0, "mColorCursor");
        requireRange(count > 0 && count <= length() - mColorCursor, "count");

        Edit edit = mPendingEdits.peek();
        if (edit != null) {
            // The color cursor shouldn't have reached the edit yet.
            requireRange(mColorCursor <= edit.start(), "mColorCursor");

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
        requireTrue(!isColorCursorDormant(), "this");
        requireRange(count > 0, "count");

        Edit edit = mPendingEdits.element();

        // Color the text before the start of the edit.
        int beforeCount = edit.start() - mColorCursor;
        // addColoring() expects the count to be greater than 0.
        if (beforeCount > 0) {
            addColoring(color, beforeCount);
            count -= beforeCount;
        }

        if (edit.isInsertion()) {
            // We haven't parsed the inserted text so we can't assume anything about how it should be colored.
            // Skip it and leave it white.
            mColorCursor += edit.count();
        } else {
            // The edit's a deletion.
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
            // Finish the portion of the coloring after the edit, potentially handling other edits
            // along the way via recursion.
            addColoring(color, count);
        }
    }

    private void advanceColorCursor(int count) {
        requireTrue(!isColorCursorDormant(), "this");

        mColorCursor += count;
    }

    private boolean isColorCursorDormant() {
        return mColorCursor == DORMANT_COLOR_CURSOR;
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
        requireTrue(!isColorCursorDormant(), "this");
        requireRange(start >= 0, "start");
        requireRange(count > 0, "count");

        if (start < mColorCursor) {
            // Rewind the color cursor by the number of chars deleted before it.
            int beforeCount = Math.min(mColorCursor - start, count);
            mColorCursor -= beforeCount;
        }

        int end = start + count;
        if (end > mColorCursor) {
            // Find the number of chars deleted after the color cursor.
            // When we arrive at the start of that deleted region in the future, throw away the parts
            // of the colorings that land inside that region.
            int afterStart = Math.max(mColorCursor, start);
            int afterCount = end - afterStart;

            requireTrue(afterCount == Math.min(end - mColorCursor, count), "afterCount");
            mPendingEdits.addDeletion(afterStart, afterCount);
        }
    }

    private void registerInsertion(int start, int count) {
        requireTrue(!isColorCursorDormant(), "this");
        requireRange(start >= 0, "start");
        requireRange(count > 0, "count");

        if (start <= mColorCursor) {
            // Synchronize the color cursor with the updated text.
            mColorCursor += count;
            return;
        }

        // In the future, skip coloring the inserted text and leave it white.
        mPendingEdits.addInsertion(start, count);
    }

    private void removeSpans(Object[] spans) {
        for (Object span : spans) {
            removeSpan(span);
        }
    }
}
