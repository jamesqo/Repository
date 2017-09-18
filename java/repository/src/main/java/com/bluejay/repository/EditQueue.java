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

    public int size() {
        return mList.size();
    }

    private void add(Edit edit) {
        Verify.isTrue(edit != null);

        int insertIndex = getInsertIndex(edit);
        int diff = edit.diff();

        if (insertIndex != 0) {
            Edit previous = get(insertIndex - 1);
            Verify.isTrue(previous.start() <= edit.start());

            if (shouldMergeWithPreviousEdit(edit, previous)) {
                // Instead of adding a new edit, we can merge this one with the previous one, since they overlap or are adjacent.
                previous.setCount(previous.count() + edit.count());
                return;
            }
        }

        if (!edit.isInsertion()) {
            // This edit's a deletion.
            int deletionCount = edit.count();
            int unprocessed = edit.count();

            // First, consume the previous edit if it is an insertion and it overlaps with this deletion.
            // If the previous edit is a deletion, merging with it is handled above, so we shouldn't reach here
            // if the deletions overlap.
            int i = insertIndex;
            if (insertIndex != 0) {
                Edit previous = get(insertIndex - 1);

                if (previous.isInsertion() && previous.contains(edit.start(), Bounds.INCLUSIVE_EXCLUSIVE)) {
                    int overlapCount = Math.min(unprocessed, previous.end() - edit.start());
                    if (overlapCount == previous.count()) {
                        mList.remove(insertIndex - 1);
                        insertIndex--;
                        i--;
                    } else {
                        previous.setCount(previous.count() - overlapCount);
                    }

                    deletionCount -= overlapCount;
                    unprocessed -= overlapCount;
                }
            }

            // Take care of subsequent insertions and deletions.
            for (; unprocessed > 0 && i < size(); i++) {
                Verify.isTrue(i >= 0);
                Edit previous = i > 0 ? get(i - 1) : null;
                Edit current = get(i);

                // Take care of the 'unmarked' region between the previous edit's end and the current one's start.
                int previousEnd = previous != null ? previous.visualEnd() : edit.start();
                int unmarked = current.start() - previousEnd;

                // We don't check <= here so that if 'edit' ends exactly where another deletion starts, the two get merged.
                if (unprocessed < unmarked) {
                    break;
                }

                unprocessed -= unmarked;

                // Consume the current edit if it is an insertion.
                // Merge with the current edit if it is a deletion.
                if (current.isInsertion()) {
                    int overlapCount = Math.min(unprocessed, current.count());
                    if (overlapCount == current.count()) {
                        mList.remove(i);
                        i--;
                    } else {
                        current.setCount(current.count() - overlapCount);
                    }

                    deletionCount -= overlapCount;
                    unprocessed -= overlapCount;
                } else {
                    // 'current' is a deletion. Merge it with 'edit'.
                    deletionCount += current.count();
                    mList.remove(i);
                    i--;
                }
            }

            edit.setCount(deletionCount);
        }

        adjustEdits(insertIndex, diff);
        mList.add(insertIndex, edit);
    }

    private void adjustEdits(int startIndex, int diff) {
        for (int i = startIndex; i < size(); i++) {
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
        for (int i = 0; i < size(); i++) {
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
        return size();
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
