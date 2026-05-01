using System.Collections;
using System.Collections.Generic;

namespace Aori.DSA.Generic
{
    public struct Matrix2Enumerator<T> :
        IEnumerator<T>
    {
        private readonly T[] r_elements;
        private readonly int r_length;
        private int m_index;

        public T Current => r_elements[m_index];
        object IEnumerator.Current => Current;

        public Matrix2Enumerator(T[] elements)
        {
            r_elements = elements;
            r_length = r_elements.Length;
            m_index = -1;
        }

        public bool MoveNext()
            => ++m_index < r_length;

        public void Reset()
            => m_index = -1;

        public void Dispose()
        { }
    }
}