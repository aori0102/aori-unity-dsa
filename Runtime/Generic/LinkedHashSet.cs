using System;
using System.Collections.Generic;
using System.Linq;

namespace Aori.DSA.Generic
{
    public class LinkedHashSet<T>
    {
        public int Count => r_itemList.Count;

        private readonly HashSet<T> r_itemHashSet = new();
        private readonly List<T> r_itemList = new();

        public bool Add(T item)
        {
            if (!r_itemHashSet.Add(item))
            {
                return false;
            }

            r_itemList.Add(item);
            return true;
        }

        public bool Remove(T item)
        {
            if (!r_itemHashSet.Remove(item))
            {
                return false;
            }

            r_itemList.Remove(item);
            return true;
        }

        public bool Contains(T item)
        {
            return r_itemHashSet.Contains(item);
        }

        public T RemoveAt(int index)
        {
            if (index >= r_itemList.Count)
            {
                throw new ArgumentOutOfRangeException($"Index {index} exceeds the collection size!");
            }

            var item = r_itemList[index];
            r_itemList.RemoveAt(index);
            r_itemHashSet.Remove(item);

            return item;
        }

        public T Last()
        {
            return r_itemList.LastOrDefault();
        }

        public T RemoveLast()
        {
            if (Count == 0)
            {
                return default;
            }

            var item = r_itemList.Last();
            r_itemList.Remove(item);
            r_itemHashSet.Remove(item);

            return item;
        }

        public T this[int index] => r_itemList[index];
    }
}