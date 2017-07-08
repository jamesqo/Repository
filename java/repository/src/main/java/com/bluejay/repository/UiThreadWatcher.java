package com.bluejay.repository;

import android.text.Editable;
import android.text.SpanWatcher;
import android.text.Spannable;
import android.text.TextWatcher;

import java.util.ArrayList;
import java.util.List;

public class UiThreadWatcher implements SpanWatcher, TextWatcher {
    private final SpanWatcher spanWatcher;
    private final TextWatcher textWatcher;

    private List<Tuple4<Spannable, Object, Integer, Integer>> onSpanAddedArgs;
    private List<Tuple4<Spannable, Object, Integer, Integer>> onSpanRemovedArgs;
    private List<Tuple6<Spannable, Object, Integer, Integer, Integer, Integer>> onSpanChangedArgs;
    private List<Tuple4<CharSequence, Integer, Integer, Integer>> beforeTextChangedArgs;
    private List<Tuple4<CharSequence, Integer, Integer, Integer>> onTextChangedArgs;
    private List<Editable> afterTextChangedArgs;

    private UiThreadWatcher(Object span) {
        this.spanWatcher = span instanceof SpanWatcher ? (SpanWatcher) span : null;
        this.textWatcher = span instanceof TextWatcher ? (TextWatcher) span : null;

        this.onSpanAddedArgs = new ArrayList<>();
        this.onSpanRemovedArgs = new ArrayList<>();
        this.onSpanChangedArgs = new ArrayList<>();
        this.beforeTextChangedArgs = new ArrayList<>();
        this.onTextChangedArgs = new ArrayList<>();
        this.afterTextChangedArgs = new ArrayList<>();
    }

    public static boolean canWrap(Object span) {
        return span instanceof SpanWatcher || span instanceof TextWatcher;
    }

    public static UiThreadWatcher wrap(Object span) {
        return canWrap(span) ? new UiThreadWatcher(span) : null;
    }

    public void flush() {
        if (this.spanWatcher != null) {
            this.flushToSpanWatcher();
        }
        if (this.textWatcher != null) {
            this.flushToTextWatcher();
        }
    }

    @Override
    public void onSpanAdded(final Spannable spannable, final Object o, final int i, final int i1) {
        this.onSpanAddedArgs.add(new Tuple4<>(spannable, o, i, i1));
    }

    @Override
    public void onSpanRemoved(final Spannable spannable, final Object o, final int i, final int i1) {
        this.onSpanRemovedArgs.add(new Tuple4<>(spannable, o, i, i1));
    }

    @Override
    public void onSpanChanged(final Spannable spannable, final Object o, final int i, final int i1, final int i2, final int i3) {
        this.onSpanChangedArgs.add(new Tuple6<>(spannable, o, i, i1, i2, i3));
    }

    @Override
    public void beforeTextChanged(final CharSequence charSequence, final int i, final int i1, final int i2) {
        this.beforeTextChangedArgs.add(new Tuple4<>(charSequence, i, i1, i2));
    }

    @Override
    public void onTextChanged(final CharSequence charSequence, final int i, final int i1, final int i2) {
        this.onTextChangedArgs.add(new Tuple4<>(charSequence, i, i1, i2));
    }

    @Override
    public void afterTextChanged(final Editable editable) {
        this.afterTextChangedArgs.add(editable);
    }

    private void flushToSpanWatcher() {
        assert this.spanWatcher != null;

        final SpanWatcher watcher = this.spanWatcher;

        if (!this.onSpanAddedArgs.isEmpty()) {
            final List<Tuple4<Spannable, Object, Integer, Integer>> args = this.onSpanAddedArgs;
            this.onSpanAddedArgs = new ArrayList<>();
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    for (Tuple4<Spannable, Object, Integer, Integer> tuple : args) {
                        watcher.onSpanAdded(tuple.item1, tuple.item2, tuple.item3, tuple.item4);
                    }
                }
            });
        }

        if (!this.onSpanRemovedArgs.isEmpty()) {
            final List<Tuple4<Spannable, Object, Integer, Integer>> args = this.onSpanRemovedArgs;
            this.onSpanRemovedArgs = new ArrayList<>();
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    for (Tuple4<Spannable, Object, Integer, Integer> tuple : args) {
                        watcher.onSpanRemoved(tuple.item1, tuple.item2, tuple.item3, tuple.item4);
                    }
                }
            });
        }

        if (!this.onSpanChangedArgs.isEmpty()) {
            final List<Tuple6<Spannable, Object, Integer, Integer, Integer, Integer>> args = this.onSpanChangedArgs;
            this.onSpanChangedArgs = new ArrayList<>();
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    for (Tuple6<Spannable, Object, Integer, Integer, Integer, Integer> tuple : args) {
                        watcher.onSpanChanged(tuple.item1, tuple.item2, tuple.item3, tuple.item4, tuple.item5, tuple.item6);
                    }
                }
            });
        }
    }

    private void flushToTextWatcher() {
        assert this.textWatcher != null;

        final TextWatcher watcher = this.textWatcher;

        if (!this.beforeTextChangedArgs.isEmpty()) {
            final List<Tuple4<CharSequence, Integer, Integer, Integer>> args = this.beforeTextChangedArgs;
            this.beforeTextChangedArgs = new ArrayList<>();
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    for (Tuple4<CharSequence, Integer, Integer, Integer> tuple : args) {
                        watcher.beforeTextChanged(tuple.item1, tuple.item2, tuple.item3, tuple.item4);
                    }
                }
            });
        }

        if (!this.onTextChangedArgs.isEmpty()) {
            final List<Tuple4<CharSequence, Integer, Integer, Integer>> args = this.onTextChangedArgs;
            this.onTextChangedArgs = new ArrayList<>();
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    for (Tuple4<CharSequence, Integer, Integer, Integer> tuple : args) {
                        watcher.onTextChanged(tuple.item1, tuple.item2, tuple.item3, tuple.item4);
                    }
                }
            });
        }

        if (!this.afterTextChangedArgs.isEmpty()) {
            final List<Editable> args = this.afterTextChangedArgs;
            this.afterTextChangedArgs = new ArrayList<>();
            Threading.postToUiThread(new Runnable() {
                @Override
                public void run() {
                    for (Editable e : args) {
                        watcher.afterTextChanged(e);
                    }
                }
            });
        }
    }
}
