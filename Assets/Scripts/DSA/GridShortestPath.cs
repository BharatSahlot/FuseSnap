using System;
using System.Collections.Generic;

namespace Game.DSA
{
	public static class GridShortestPath
	{
		private class Node : IComparable
		{
			public int r, c, d, pr, pc;

            public int CompareTo(object obj)
            {
				Node x = obj as Node;
				if(d < x.d) return -1;
				else if(d == x.d) return 1;
				return 0;
            }
        }

		private static int[] _dx = { 1, 0, -1, 0 };
		private static int[] _dy = { 0, 1, 0, -1 };

		// overlapping isnt so simple to fix.
		// imagine a wire overlapping another wire, but because of overlapping it is also overlapping another wire visually.
		// many edge cases.
        public static List<(int,int)> GetShortestPath(int[,] grid, (int r, int c) start, (int r, int c) end)
		{
			var q = new SortedSet<Node>();
			q.Add(new Node { r = start.r, c = start.c, d = 0, pr = start.r, pc = start.c });
			
			var visited = new HashSet<(int r, int c)>();
			var parent = new Dictionary<(int r, int c), (int r, int c)>();
			while(q.Count > 0)
			{
				Node node = q.Min;
				q.Remove(q.Min);

				int r = node.r, c = node.c, d = node.d;
				if(visited.Contains((r, c))) continue;
				
				visited.Add((r, c));
				parent[(r, c)] = (node.pr, node.pc);
				if(r == end.r && c == end.c) break;

				for(int i = 0; i < 4; ++i)
				{
					int nr = r + _dx[i];
					int nc = c + _dy[i];

					// if out of range then continue
					if(nr < 0 || nr >= grid.GetLength(0) || nc < 0 || nc >= grid.GetLength(1)) continue;
					if(visited.Contains((nr, nc))) continue; // already found shortest path
					q.Add(new Node { r = nr, c = nc, d = d + grid[nr, nc], pr = r, pc = c });
				}
			}
			if(!parent.ContainsKey(end)) return null;

			// reconstruct path
			List<(int, int)> path = new List<(int, int)>();
			var vert = end;
			do
			{
				path.Add(vert);
				vert = parent[vert];
			} while(vert != parent[vert]);
			path.Add(vert);
			return path;
		}
	}
}
