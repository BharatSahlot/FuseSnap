using System.Collections.Generic;
using System.Linq; // :(
using UnityEngine;

namespace Game.DSA
{
	/// Union Find Disjoint Set ( UFDS ) structure with path compression.
	public struct UnionFind
	{
		private int _n;
		private List<int> _par, _compressed, _rank;
		private bool _dirty;
		
		public int Size => _n;
		public int Components { get; private set; }
		// public int ComponentsG1 { get; private set; } // Components with size greater than 1
		public int GetComponentSize(int i) => _rank[FindSet(i)];

		public UnionFind(int sz)
		{
			_n = Components = sz;
			// ComponentsG1 = 0;
			_par = new List<int>();
			_compressed = new List<int>();
			_rank = new List<int>();
			_dirty = false;
			for(int i = 0; i < sz; ++i)
			{
				_par.Add(i);
				_compressed.Add(i);
				_rank.Add(1);
			}
		}

		public int FindSet(int i)
		{
			if(_par[i] != i) 
			{
				_par[i] = FindSet(_par[i]);
				_dirty = true;
			}
			return _par[i];
		}


		public void UnionSet(int i, int j)
		{
			i = FindSet(i);
			j = FindSet(j);

			if(i == j) return;
			
			if(i > j) Helper.Swap(ref i, ref j);
			_par[j] = i;

			_rank[i] += _rank[j];
			Components--;
			_dirty = true;
		}

		public void PathCompress()
		{
			for(int i = 0; i < _n; ++i) FindSet(i);
		}

        public int Add()
        {
			_par.Add(_n++);
			_rank.Add(1);
			Components++;
			_dirty = true; // dont need to set dirty, just add _n - 1 to end of _compressed
			return _n - 1;
        }

        public void Reset(int id)
        {
			_rank[FindSet(id)]--;
			_rank[id] = 1;
			_par[id] = id;
			Components++;
			_dirty = true;
        }

		/// Returns compressed path. Only compresses when the actual graph changes.
        public int FindSetCompressed(int id)
        {
			if(_dirty)
			{
				PathCompress();
				_dirty = false;
				
				_compressed = _par.ToList();
				_compressed.Sort();
				_compressed = _compressed.Distinct().ToList();

			//	string s = "UFDS: [";
			//	_par.ForEach(i => s += $" {i},");
			//	s += "] Compressed UFDS: [";
			//	_compressed.ForEach(i => s += $" {i},");
			//	s += "]";
			//	Debug.Log(s);
			}
			return _compressed.BinarySearch(_par[id]);
        }
    }
}
