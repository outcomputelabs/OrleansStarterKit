using System;
using System.Collections.Generic;

namespace Grains
{
    /// <summary>
    /// Extends the <see cref="Queue{T}"/> with additional utility methods.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Enqueues the given object.
        /// If the new queue size would be greater than <paramref name="capacity"/> then
        /// this method makes room for the new item by dequeueing until there is a spot.
        /// </summary>
        /// <typeparam name="T">The type of object to enqueue.</typeparam>
        /// <param name="queue">The queue to apply this extension on.</param>
        /// <param name="item">The object to enqueue.</param>
        /// <param name="capacity">
        /// The maximum queue capacity to enforce.
        /// If the new queue size would be greater than this value then items are dequeued until there is space for the new item.
        /// This value must greater than zero.
        /// </param>
        public static void Enqueue<T>(this Queue<T> queue, T item, int capacity)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), capacity, $"Capacity is {capacity} but must be greater than zero.");

            // make room for the new item
            while (queue.Count > capacity - 1)
            {
                queue.Dequeue();
            }

            // enqueue the new item
            queue.Enqueue(item);
        }

        /// <summary>
        /// Enqueues the given enumeration of objects.
        /// If the new queue size would be greater than <paramref name="capacity"/> then this method makes room as needed.
        /// </summary>
        /// <typeparam name="T">The type of object to enqueue.</typeparam>
        /// <param name="queue">The queue to apply this extension on.</param>
        /// <param name="items">The enumeration of objects to enqueue.</param>
        /// <param name="capacity">
        /// The maximum queue capacity to enforce.
        /// If the new queue size would be greater than this value then items are dequeued until there is space for the new item.
        /// This value must greater than zero.
        /// </param>
        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items, int capacity)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item, capacity);
            }
        }
    }
}
