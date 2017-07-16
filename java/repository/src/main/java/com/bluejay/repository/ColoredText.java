package com.bluejay.repository;

import android.text.SpannableStringBuilder;
import android.text.style.*;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class ColoredText extends SpannableStringBuilder {
    private int index;

    public ColoredText(String rawText) {
        super(rawText);
    }

    public int colorWith(ColoringList colorings) {
        int processed = colorings.count();

        for (int i = 0; i < colorings.count(); i++) {
            long coloring = colorings.get(i);
            int color = getColor(coloring);
            int count = getCount(coloring);

            if (this.index + count <= this.length()) {
                this.advance(color, count);
                continue;
            }

            // This coloring extends past the end of the text.
            // Split it into 2 regions, `before` and `after`.
            // Update the coloring to only include `after` for the next ColoredText.
            int before = this.length() - this.index;
            int after = count - before;

            if (before > 0) {
                colorings.set(i, makeColoring(color, after));
                this.advance(color, before);
            }

            // Don't count the current coloring. We want the next ColoredText to see it.
            processed = i;
            break;
        }

        return processed;
    }

    private void advance(int color, int count) {
        assert count > 0;

        Object span = new ForegroundColorSpan(color);
        this.setSpan(span, this.index, this.index + count, SPAN_INCLUSIVE_EXCLUSIVE);
        this.index += count;
    }

    private static int getColor(long coloring) {
        return (int)(coloring >> 32);
    }

    private static int getCount(long coloring) {
        return (int)coloring;
    }

    private static long makeColoring(int color, int count) {
        assert count > 0; // Negative numbers are sign-extended from int -> long
        return ((long)color << 32) | count;
    }
}
