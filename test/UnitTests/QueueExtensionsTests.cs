using Grains;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{
    public class QueueExtensionsTests
    {
        [Fact]
        public void Queue_Enqueue_Respects_Capacity()
        {
            // arrange
            var queue = new Queue<int>();
            var capacity = 3;

            // act
            queue.Enqueue(101, capacity);
            queue.Enqueue(102, capacity);
            queue.Enqueue(103, capacity);
            queue.Enqueue(104, capacity);

            // assert
            var count = queue.Count;
            var items = new List<int>();
            while (queue.Count > 0)
            {
                items.Add(queue.Dequeue());
            }

            Assert.Equal(capacity, count);
            Assert.Equal(capacity, items.Count);
            Assert.Equal(102, items[0]);
            Assert.Equal(103, items[1]);
            Assert.Equal(104, items[2]);
        }
    }
}
