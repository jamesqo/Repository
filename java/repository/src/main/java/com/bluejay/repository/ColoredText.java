package com.bluejay.repository;

import android.support.annotation.ColorInt;
import android.text.SpannableStringBuilder;
import android.text.style.*;

import java.util.LinkedList;
import java.util.Queue;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class ColoredText extends SpannableStringBuilder {
    private final Queue<TextChange> pendingChanges;

    private int index;

    public ColoredText(String rawText) {
        super(rawText);
        this.pendingChanges = new LinkedList<>();
    }

    public void colorWith(ColoringList colorings) {
        for (int i = 0; i < colorings.count(); i++) {
            long coloring = colorings.get(i);
            this.advance(getColor(coloring), getCount(coloring));
        }
    }

    @Override
    public SpannableStringBuilder replace(int start, int end, CharSequence tb, int tbstart, int tbend) {
        assert start >= 0;
        assert start <= end;

        int count = end - start;
        int tbcount = tbend - tbstart;
        if (count != tbcount) {
            // We don't want to misalign the color spans for text we haven't highlighted yet.
            this.registerChange(start, tbcount - count);
        }

        return super.replace(start, end, tb, tbstart, tbend);
    }

    private void addPendingChange(TextChange change) {
        this.pendingChanges.add(change);
    }

    private void advance(@ColorInt int color, int count) {
        assert count > 0;
        assert this.index + count <= this.length();

        TextChange nextChange = this.peekPendingChange();
        if (nextChange != null) {
            assert this.index < nextChange.start();

            if (this.index + count >= nextChange.start()) {
                this.
                // TODO
            }
        }

        Object span = new ForegroundColorSpan(color);
        this.setSpan(span, this.index, this.index + count, SPAN_INCLUSIVE_EXCLUSIVE);
        this.index += count;
    }

    @ColorInt
    private static int getColor(long coloring) {
        return (int)(coloring >> 32);
    }

    private static int getCount(long coloring) {
        return (int)coloring;
    }

    private TextChange peekPendingChange() {
        return this.pendingChanges.peek();
    }

    private void registerChange(int start, int diff) {
        assert start >= 0;
        assert diff != 0; // Positive for insertions, negative for deletions

        if (start <= this.index) {
            this.index += diff;
            return;
        }

        this.addPendingChange(new TextChange(start, diff));
    }
}
