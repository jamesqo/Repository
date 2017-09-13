package com.bluejay.repository;

import java.util.ArrayList;
import java.util.List;
import java.util.NoSuchElementException;

public class EditQueue {
    // There might be a way to improve complexity here by using a PriorityQueue. However, a plain List
    // works fine in practice, as users can't type fast enough to make n (the number of edits) really big
    // before the file is finished being highlighted.
    private final List<Edit> mList;

    public EditQueue() {
        mList = new ArrayList<>();
    }

    public void addDeletion(int start, int count) {
        add(Edit.deletion(start, count));
    }

    public void addInsertion(int start, int count) {
        add(Edit.insertion(start, count));
    }

    public Edit element() {
        Edit result = peek();
        if (result == null) {
            throw new NoSuchElementException();
        }
        return result;
    }

    public boolean isEmpty() {
        return mList.isEmpty();
    }

    public Edit peek() {
        return isEmpty() ? null : mList.get(0);
    }

    public Edit poll() {
        return isEmpty() ? null : mList.remove(0);
    }

    public Edit remove() {
        Edit result = poll();
        if (result == null) {
            throw new NoSuchElementException();
        }
        return result;
    }

    private void add(Edit edit) {
        Verify.isTrue(edit != null);

        int insertIndex = getInsertIndex(edit);
        if (insertIndex != 0) {
            Edit previous = mList.get(insertIndex - 1);
            Verify.isTrue(previous.start() <= edit.start());
            if (previous.isInsertion() == edit.isInsertion() &&
                    previous.containsInclusive(edit.start())) {
                // Instead of adding a new edit, we can merge this one with the previous one, since they overlap or are adjacent.
                // TODO: Check this, add comments
                if (edit.start() < previous.start()) {
                    previous.setStart(edit.start());
                }
                int end = Math.max(edit.end(), previous.end());
                previous.setCount(end - previous.start());
                return;
            }
        }

        mList.add(insertIndex, edit);
    }

    private int getInsertIndex(Edit edit) {
        Verify.isTrue(edit != null);
        Verify.isTrue(!mList.contains(edit));

        // mList should be sorted by each edit's start index.
        for (int i = 0; i < mList.size(); i++) {
            Edit e = mList.get(i);
            if (i != 0) {
                Edit previous = mList.get(i - 1);
                Verify.isTrue(previous.start() <= e.start());
            }
            if (e.start() > edit.start()) {
                return i;
            }
        }

        // All edits had a start equal to or less than the new one. Insert the new one at the end of the list.
        return mList.size();
    }
}
