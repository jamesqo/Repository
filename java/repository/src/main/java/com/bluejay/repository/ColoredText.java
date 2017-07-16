package com.bluejay.repository;

import android.support.annotation.ColorInt;
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

    public void colorWith(ColoringList colorings) {
        for (int i = 0; i < colorings.count(); i++) {
            long coloring = colorings.get(i);
            this.advance(getColor(coloring), getCount(coloring));
        }
    }

    private void advance(@ColorInt int color, int count) {
        assert count > 0;
        assert this.index + count <= this.length();

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
}
