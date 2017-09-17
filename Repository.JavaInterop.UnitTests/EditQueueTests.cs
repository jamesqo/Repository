using NUnit.Framework;
using static Repository.JavaInterop.Edit;

namespace Repository.JavaInterop.UnitTests
{
    [TestFixture]
    public class EditQueueTests
    {
        [Test]
        public void AddDeletion_Adjacent_MergesDeletions_MergeeAfter()
        {
            var queue = new EditQueue();
            queue.AddDeletion(0, 3);
            queue.AddDeletion(0, 2);

            var expected = new[] { Deletion(0, 5) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_Adjacent_MergesDeletions_MergeeBefore()
        {
            var queue = new EditQueue();
            queue.AddDeletion(2, 3);
            queue.AddDeletion(1, 1);

            var expected = new[] { Deletion(1, 4) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_Adjacent_MergesDeletions_MergeeBeforeAndAfter()
        {
            var queue = new EditQueue();
            queue.AddDeletion(2, 3);
            queue.AddDeletion(1, 2);

            var expected = new[] { Deletion(1, 5) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_NotAdjacent_DoesNotMergeDeletions()
        {
            var queue = new EditQueue();
            queue.AddDeletion(0, 3);
            queue.AddDeletion(1, 2);

            var expected = new[] { Deletion(0, 3), Deletion(1, 2) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_Overlapping_ShortensInsertion()
        {
            var queue = new EditQueue();
            queue.AddInsertion(0, 5);
            queue.AddDeletion(3, 4);

            var expected = new[] { Insertion(0, 3), Deletion(3, 2) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_EncompassesInsertion_RemovesInsertion()
        {
            var queue = new EditQueue();
            queue.AddInsertion(1, 2);
            queue.AddDeletion(0, 4);

            var expected = new[] { Deletion(0, 2) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_EncompassesMultipleDeletionsAndInsertions()
        {
            var queue = new EditQueue();
            queue.AddInsertion(10, 20);
            queue.AddInsertion(50, 50);
            queue.AddDeletion(35, 50);
            queue.AddDeletion(2, 4);

            queue.AddDeletion(1, 200);

            var expected = new[] { Deletion(1, 200 + 50 + 4 - 20 - 50) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_EditsArePrioritizedByStartIndex()
        {
            var queue = new EditQueue();
            queue.AddDeletion(7, 6);
            queue.AddDeletion(0, 6);

            var expected = new[] { Deletion(0, 6), Deletion(1, 6) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddDeletion_EditsAreAhead_AdjustsStartIndex()
        {
            var queue = new EditQueue();
            queue.AddDeletion(40, 30);
            queue.AddInsertion(10, 15);

            queue.AddDeletion(3, 4);

            var expected = new[] { Deletion(3, 4), Insertion(6, 15), Deletion(51, 30) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddInsertion_MergesInsertions()
        {
            var queue = new EditQueue();
            queue.AddInsertion(0, 5);
            queue.AddInsertion(3, 4);

            var expected = new[] { Insertion(0, 9) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddInsertion_EditsArePrioritizedByStartIndex()
        {
            var queue = new EditQueue();
            queue.AddInsertion(1, 6);
            queue.AddInsertion(0, 6);

            var expected = new[] { Insertion(0, 6), Insertion(7, 6) };
            Assert.AreEqual(expected, queue);
        }

        [Test]
        public void AddInsertion_EditsAreAhead_AdjustsStartIndex()
        {
            var queue = new EditQueue();
            queue.AddInsertion(90, 80);
            queue.AddDeletion(70, 10);

            queue.AddInsertion(15, 20);

            var expected = new[] { Insertion(15, 20), Deletion(90, 10), Insertion(100, 80) };
            Assert.AreEqual(expected, queue);
        }
    }
}
