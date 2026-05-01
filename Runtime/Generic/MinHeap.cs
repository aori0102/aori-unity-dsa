using System;
using System.Collections;
using System.Collections.Generic;

namespace Aori.DSA.Generic
{
    public class MinHeap<T> : IEnumerable<T> where T : IComparable<T>
    {
        private readonly List<T> r_elements = new();

        public int Count => r_elements.Count;
        public bool IsEmpty => r_elements.Count == 0;

        public void Clear()
            => r_elements.Clear();

        public T Peek()
            => r_elements.Count == 0 ? throw new InvalidOperationException("Heap is empty") : r_elements[0];

        public bool Contains(T item)
            => r_elements.Contains(item);

        public IEnumerator<T> GetEnumerator()
            => r_elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(T item)
        {
            r_elements.Add(item);
            HeapifyUp(r_elements.Count - 1);
        }

        public T Pop()
        {
            if (r_elements.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }

            var result = r_elements[0];
            r_elements[0] = r_elements[^1];
            r_elements.RemoveAt(r_elements.Count - 1);
            HeapifyDown(0);
            return result;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                var parent = (index - 1) / 2;
                if (r_elements[index].CompareTo(r_elements[parent]) >= 0)
                {
                    break;
                }

                (r_elements[index], r_elements[parent]) = (r_elements[parent], r_elements[index]);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            var last = r_elements.Count - 1;

            while (true)
            {
                var left = 2 * index + 1;
                var right = 2 * index + 2;
                var smallest = index;

                if (left <= last && r_elements[left].CompareTo(r_elements[smallest]) < 0)
                {
                    smallest = left;
                }

                if (right <= last && r_elements[right].CompareTo(r_elements[smallest]) < 0)
                {
                    smallest = right;
                }

                if (smallest == index)
                {
                    break;
                }

                (r_elements[index], r_elements[smallest]) = (r_elements[smallest], r_elements[index]);
                index = smallest;
            }
        }
    }
}