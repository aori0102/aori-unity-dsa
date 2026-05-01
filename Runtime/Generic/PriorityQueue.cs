using System;
using System.Collections;
using System.Collections.Generic;

namespace Aori.DSA.Generic
{
    /// <summary>
    /// A priority queue implemented with max heap. which means the element with the greatest value
    /// (top priority) will always be in front.
    /// </summary>
    /// <typeparam name="T">Comparable type.</typeparam>
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private readonly MaxHeap<T> r_heap = new();

        public int Count => r_heap.Count;
        public bool IsEmpty => r_heap.IsEmpty;

        /// <summary>
        /// Clear all content from this queue.
        /// </summary>
        public void Clear()
            => r_heap.Clear();

        /// <summary>
        /// Check the given item's presence within the queue.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns><c>true</c> when the item is present within the queue, otherwise <c>false</c>.
        /// </returns>
        public bool Contains(T item)
            => r_heap.Contains(item);

        /// <summary>
        /// Add an item to the priority queue.
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void Enqueue(T item)
            => r_heap.Add(item);

        /// <summary>
        /// Peek the first item of the queue. This does not remove the item from the queue.
        /// </summary>
        /// <returns>The peeked item if succeeded.</returns>
        /// <exception cref="InvalidOperationException">When the priority queue is empty.</exception>
        public T Peek()
            => r_heap.IsEmpty ? throw new InvalidOperationException("Priority Queue is empty.") : r_heap.Peek();

        public IEnumerator<T> GetEnumerator()
            => r_heap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Get the first item of the queue.
        /// </summary>
        /// <returns>The first item of the queue if succeeded.</returns>
        /// <exception cref="InvalidOperationException">When the priority queue is empty.</exception>
        public T Dequeue()
        {
            if (r_heap.IsEmpty)
            {
                throw new InvalidOperationException("Priority Queue is empty.");
            }

            var element = r_heap.Pop();
            return element;
        }

        /// <summary>
        /// Try getting the first item of the queue.
        /// </summary>
        /// <param name="item">The dequeued item, or its <c>default</c> value if the priority
        /// queue is empty.</param>
        /// <returns><c>true</c> if an item is dequeued, or <c>false</c> if the priority queue is
        /// empty.</returns>
        public bool TryDequeue(out T item)
        {
            if (r_heap.IsEmpty)
            {
                item = default;
                return false;
            }

            item = r_heap.Pop();
            return true;
        }

        /// <summary>
        /// Try peeking the first item of the queue. This does not remove the item from the queue.
        /// </summary>
        /// <param name="item">The peeked item, or its <c>default</c> value if failed.</param>
        /// <returns><c>true</c> if an item is peeked, or <c>false</c> if the queue is empty.</returns>
        public bool TryPeek(out T item)
        {
            if (r_heap.IsEmpty)
            {
                item = default;
                return false;
            }

            item = r_heap.Peek();
            return true;
        }
    }
}