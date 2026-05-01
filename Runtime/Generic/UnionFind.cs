namespace Aori.DSA.Generic
{
    public sealed class UnionFind
    {
        private readonly int[] _parents;

        public UnionFind(int size)
        {
            _parents = new int[size];
            for (var i = 0; i < size; i++)
            {
                _parents[i] = i;
            }
        }

        public int Find(int i)
        {
            while (true)
            {
                if (_parents[i] == i)
                {
                    return i;
                }

                i = _parents[i];
            }
        }

        public int Union(int i, int j)
        {
            var iRep = Find(i);
            var jRep = Find(j);
            
            _parents[iRep] = jRep;
            return jRep;
        }
    }
}