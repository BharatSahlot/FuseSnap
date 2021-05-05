using System.Collections.Generic;
using System.Linq; // :(
using UnityEngine;

namespace Game.DSA
{
	/// Union Find Disjoint Set ( UFDS ) structure with path compression.
	public struct UnionFind
	{
		private int _n;
		private int[] _par;

		public int Size { get { return _n; } }

		public UnionFind(int sz)
		{
			_n = sz;
			_par = new int[sz];	
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
			if(i < j) _par[i] = j;
			else _par[j] = i;
		}

		// not good
		public List<int> Compress()
		{
			for(int i = 0; i < _n; ++i) _par[i] = FindSet(i);
			
			var sorted = new List<int>(_par);
			sorted.Sort();
			sorted = sorted.Distinct().ToList();

		//	string s = "[ ";
		//	sorted.ForEach((j) => { s += j.ToString() + " "; });
		//	s += "] [ ";
		//	_par.ToList().ForEach((j) => { s += j.ToString() + " "; });
		//	Debug.Log(s + "]");

			var res = new List<int>(_par);
			for(int i = 0; i < _n; ++i)
			{
				res[i] = sorted.BinarySearch(_par[i]);
			}
			return res;
		}
	}
}
