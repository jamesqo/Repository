using NUnit.Framework;
using static Repository.JavaInterop.Edit;

namespace Repository.JavaInterop.UnitTests
{
    [TestFixture]
    public class EditQueueTests
    {
        [Test]
        public void AddDeletion_MergesDeletions()
        {
            var queue = new EditQueue();
            queue.AddDeletion(0, 3);
            queue.AddDeletion(0, 2);

            var expected = new[] { Deletion(0, 5) };
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
        public void AddDeletion_Encompassing_RemovesInsertion()
        {
            var queue = new EditQueue();
            queue.AddInsertion(1, 2);
            queue.AddDeletion(0, 4);

            var expected = new[] { Deletion(0, 2) };
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
