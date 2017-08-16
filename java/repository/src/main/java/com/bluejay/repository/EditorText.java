package com.bluejay.repository;

import android.support.annotation.ColorInt;
import android.text.SpannableStringBuilder;
import android.text.style.*;

import java.util.ArrayList;
import java.util.HashSet;

// It would be preferable to implement Editable and wrap a SpannableStringBuilder,
// instead of extending it directly. However, that causes the EditText to act glitchy.
// See https://stackoverflow.com/q/45125759/4077294 for more info.
public class EditorText extends SpannableStringBuilder {
    private final HashSet<Object> systemSpans;

    private int index;

    public EditorText(String rawText) {
        super(rawText);

        this.systemSpans = new HashSet<>();
    }

    public void clearColorSpans() {
        this.index = 0; // TODO: This is a temp hack. Remove when index field no longer exists.
                        // Note that this can cause problems if run while another update is still running.

        ArrayList<SpanInfo> systemSpanInfos = new ArrayList<>();
        for (Object span : this.systemSpans) {
            systemSpanInfos.add(new SpanInfo(
                    span,
                    this.getSpanStart(span),
                    this.getSpanEnd(span),
                    this.getSpanFlags(span)
            ));
        }

        super.clearSpans();

        for (SpanInfo info : systemSpanInfos) {
            this.setSpan(info.span, info.start, info.end, info.flags);
        }
    }

    @Override
    public void clearSpans() {
        throw new UnsupportedOperationException();
    }

    public void colorWith(ColoringList colorings) {
        for (int i = 0; i < colorings.count(); i++) {
            long coloring = colorings.get(i);
            this.advance(getColor(coloring), getCount(coloring));
        }
    }

    @Override
    public void removeSpan(Object what) {
        this.systemSpans.remove(what);
        super.removeSpan(what);
    }

    @Override
    public SpannableStringBuilder replace(int start, int end, CharSequence tb, int tbstart, int tbend) {
        // If text is deleted or inserted while the highlighter is running, make sure the regions
        // of text it highlights stays in sync with the source code.
        int count = end - start;
        int tbcount = tbend - tbstart;
        int diff = tbcount - count;
        this.index += diff;

        return super.replace(start, end, tb, tbstart, tbend);
    }

    @Override
    public void setSpan(Object what, int start, int end, int flags) {
        this.systemSpans.add(what);
        super.setSpan(what, start, end, flags);
    }

    private void advance(@ColorInt int color, int count) {
        assert count > 0;
        assert this.index + count <= this.length();

        ForegroundColorSpan span = new ForegroundColorSpan(color);
        super.setSpan(span, this.index, this.index + count, SPAN_INCLUSIVE_EXCLUSIVE);
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
