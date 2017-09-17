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

            if (shouldMergeWithPreviousEdit(edit, previous)) {
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
                    // TODO: skip()? shiftRight()?
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

    private static boolean shouldMergeWithPreviousEdit(Edit edit, Edit previous) {
        Verify.isTrue(edit != null);
        Verify.isTrue(previous != null);
        Verify.isTrue(previous.start() <= edit.start());

        if (edit.isInsertion()) {
            // Suppose the user inserts 5 chars at index 3, then inserts 6 chars.
            // If the 6 chars are inserted anywhere from 3..8 inclusive, the edits can be represented with
            // a single 11-char insertion at index 3.
            return previous.isInsertion() && previous.contains(edit.start(), Bounds.INCLUSIVE_INCLUSIVE);
        }

        // The edit's a deletion.

        // Suppose the user deletes 5 chars at index 3, then deletes 6 chars.
        // Unless the second deletion also happens at index 3, the edits cannot be represented as a single
        // 11-char deletion.
        // If the user deletes 1 or more chars at index 2 for the second step, however, the two deletions
        // can still be merged. In other words, it's easy for a deletion to merge with subsequent deletions.
        // Since this function deals with merging the edit with the *previous* one, however, that isn't handled here.
        return !previous.isInsertion() && previous.start() == edit.start();
    }
}
