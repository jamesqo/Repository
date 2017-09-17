package com.bluejay.repository;

import java.util.ArrayList;
import java.util.List;
import java.util.NoSuchElementException;

import static com.bluejay.repository.Edit.Bounds;

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
        return isEmpty() ? null : get(0);
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
        adjustEdits(insertIndex, edit.diff());

        if (insertIndex != 0) {
            Edit previous = get(insertIndex - 1);
            Verify.isTrue(previous.start() <= edit.start());

            if (previous.isInsertion() == edit.isInsertion() &&
                    previous.contains(edit.start(), Bounds.INCLUSIVE_INCLUSIVE)) {
                // Instead of adding a new edit, we can merge this one with the previous one, since they overlap or are adjacent.
                // TODO: Check this, add comments
                previous.setCount(previous.count() + edit.count());
                return;
            }

            if (previous.isInsertion()) {
                Verify.isTrue(!edit.isInsertion());
                if (previous.contains(edit.start(), Bounds.INCLUSIVE_EXCLUSIVE)) {
                    int overlapStart = edit.start();
                    int overlapEnd = Math.min(previous.end(), edit.end());
                    int overlapCount = overlapEnd - overlapStart;

                    previous.setCount(previous.count() + overlapCount);
                    if (edit.count() == overlapCount) {
                        // The new deletion was fully inside the insertion. No more work to do.
                        return;
                    }

                    Verify.isTrue(overlapEnd == previous.end());
                    // TODO: skip()?
                    edit.setStart(edit.start() + overlapCount);
                    edit.setCount(edit.count() - overlapCount);
                }
            }
        }

        mList.add(insertIndex, edit);
    }

    private void adjustEdits(int startIndex, int diff) {
        for (int i = startIndex; i < mList.size(); i++) {
            Edit edit = get(i);
            edit.setStart(edit.start() + diff);
        }
    }

    private Edit get(int index) {
        return mList.get(index);
    }

    private int getInsertIndex(Edit edit) {
        Verify.isTrue(edit != null);
        Verify.isTrue(!mList.contains(edit));

        // mList should be sorted by each edit's start index.
        for (int i = 0; i < mList.size(); i++) {
            Edit e = get(i);
            if (i != 0) {
                Edit previous = get(i - 1);
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
