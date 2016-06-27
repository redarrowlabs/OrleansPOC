using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Text;

namespace Grains.Infrastructure
{
    public class HashRing<T>
    {
        private readonly T[] _nodes;
        private readonly MurmurHash3 _murmurHash3;

        public HashRing(int nodeCount, Func<int, T> nodeExpression)
        {
            _murmurHash3 = new MurmurHash3();
            _nodes = new T[nodeCount];
            for (var i = 0; i < nodeCount; i++)
            {
                _nodes[i] = nodeExpression(i);
            }
        }

        public IEnumerable<T> Nodes
        {
            get { return _nodes; }
        }

        public T GetNode(string key)
        {
            int index = Math.Abs(GetHashCode(key)) % _nodes.Length;
            return _nodes[index];
        }

        private int GetHashCode(string id)
        {
            return BitConverter.ToInt32(_murmurHash3.ComputeHash(Encoding.ASCII.GetBytes(id)), 0);
        }
    }
}