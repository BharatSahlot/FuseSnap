using System;
using System.Collections.Generic;
using System.Linq; // :(

namespace Game.DSA
{
	/// Union Find Disjoint Set ( UFDS ) structure with path compression.
	public struct UnionFind
	{
		private int _n;
		private List<int> _par;

		public int Size { get { return _n; } }
		public int Components { get; private set; }

		public UnionFind(int sz)
		{
			_n = Components = sz;
			_par = new List<int>(sz);	
			for(int i = 0; i < sz; ++i)
			{
				_par[i] = i;
			}
		}

		public int FindSet(int i)
		{
			if(_par[i] != i) _par[i] = FindSet(_par[i]);
			return _par[i];
		}

		public void UnionSet(int i, int j)
		{
			i = FindSet(i);
			j = FindSet(j);

			if(i == j) return;
			if(i > j) _par[i] = j;
			else _par[j] = i;
			Components--;
		}

		public void PathCompress()
		{
			for(int i = 0; i < _n; ++i) _par[i] = FindSet(i);
		}

		// not good
		public List<int> Compress()
		{
			PathCompress();

			var sorted = _par;
			sorted.Sort();
			sorted = sorted.Distinct().ToList();

			var res = new List<int>(_par);
			for(int i = 0; i < _n; ++i)
			{
				res[i] = sorted.BinarySearch(_par[i]);
			}
			return res;
		}

        public int Add()
        {
			_par.Add(Size);
			_n++;
			Components++;
			return Size + 1;
        }
    }
}
