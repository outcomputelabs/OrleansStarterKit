using System.Collections.Generic;

namespace Grains
{
    public static class QueueExtensions
    {
        public static void Enqueue<T>(this Queue<T> queue, T item, int capacity)
        {
            queue.Enqueue(item);
            while (queue.Count > capacity)
            {
                queue.Dequeue();
            }
        }
    }
}
