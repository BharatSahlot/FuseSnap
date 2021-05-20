using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game.Graphics 
{
	public class CircuitGrid : MonoBehaviour
	{
		private static CircuitGrid _instance = null;
		public static CircuitGrid Instance => _instance;

		[SerializeField] private float _squareToScreenWidthRatio = 0.01f;
		[SerializeField] private int _filledSquareCost = 2; // cost of putting wire on top of a filled square
		[SerializeField] private int _filledNeighbourCost = 1; // cost of putting wire on top of a square which has a filled neightbour
		[SerializeField] private LayerMask _circuitLayerMask;
		
        /// Square size in world space
		public float SquareSize { get; private set; }
		public int R { get; private set; }
		public int C { get; private set; }

		private int[,] _grid;

		// ^
		// |
		// | Column
		// |--------> Row
		private void Awake()
		{
			if(_instance != null)
			{
				Debug.LogError("Multiple CircuitGrid Instances");
			} else _instance = this;

			Camera camera = Camera.main;
			Vector3 size = camera.ViewportToWorldPoint(Vector3.zero) - 
				camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0));

			SquareSize = Mathf.Abs(size.x) * _squareToScreenWidthRatio;
			R = (int)(Mathf.Abs(size.x) / SquareSize); // number of rows is always same -> 1 / _squareToScreenWidthRatio
			C = (int)(Mathf.Abs(size.y) / SquareSize);
			
			_grid = new int[R, C];
		}

		private void FillSquare(int r, int c)
		{
			_grid[r, c] += _filledSquareCost;
			if(r + 1 < R) _grid[r + 1, c] += _filledNeighbourCost;
			if(r - 1 >= 0) _grid[r - 1, c] += _filledNeighbourCost;
			if(c + 1 < C) _grid[r, c + 1] += _filledNeighbourCost;
			if(c - 1 >= 0) _grid[r, c - 1] += _filledNeighbourCost;
		}
     
		public void AddComponent(Bounds worldSpace, Bounds localSpace, Transform obj)
		{
			Vector3 center = worldSpace.center;
			Vector3 extents = worldSpace.extents;
			for(float x = center.x - extents.x; x <= center.x + extents.x; x += SquareSize)
			{
				for(float y = center.y - extents.y; y <= center.y + extents.y; y += SquareSize)
				{
                    var point = new Vector3(x, y, 0);
                    var (r, c) = GetSquareAtWorldPosition(point);
                    point = GetSquareWorldPosition(r, c);
                    if(localSpace.Contains(obj.InverseTransformPoint(point)))
                    {
                        FillSquare(r, c);
				    }
                }
			}
		}

		public bool DrawWire(Vector3 a, Vector3 b, LineRenderer line)
		{
			var start = GetSquareAtWorldPosition(a);
			var end = GetSquareAtWorldPosition(b);
			List<(int r, int c)> path = DSA.GridShortestPath.GetShortestPath(_grid, start, end);
			if(path == null) return false;
			foreach((int r, int c) in path)
			{
				FillSquare(r, c);
			}

			// try using 2d version
			List<Vector3> positions = new List<Vector3>();
			Vector2 dir = Vector2.zero;
			for(int i = 0; i < path.Count - 1; ++i)
			{
				Vector2 nd = new Vector2(path[i].r - path[i + 1].r, path[i].c - path[i + 1].c);
				if(nd != dir) positions.Add(GetSquareWorldPosition(path[i].r, path[i].c));
				dir = nd;
			}
			positions.Add(GetSquareWorldPosition(path[path.Count - 1].Item1, path[path.Count - 1].Item2));
			line.positionCount = positions.Count;
			line.SetPositions(positions.ToArray());
			return true;
		}

		// this is working
		public (int r, int c) GetSquareAtWorldPosition(Vector3 worldPosition)
		{
			worldPosition.x += (R * SquareSize / 2.0f); // - (SquareSize / 2.0f);
			worldPosition.y += (C * SquareSize / 2.0f); // - (SquareSize / 2.0f);
			int r = (int)(worldPosition.x / SquareSize); 
			int c = (int)(worldPosition.y / SquareSize); 
			return (r, c);
		}

		// this is working
		public Vector3 GetSquareWorldPosition(int r, int c)
		{
			Vector3 res = new Vector3();
			// square world position = center + row * size - something because worldposition 0,0 is at center of screen
			res.x = SquareSize + r * SquareSize - (R * SquareSize / 2.0f);
			res.y = SquareSize + c * SquareSize - (C * SquareSize / 2.0f);
			return res;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			for(int r = 0; r < R; ++r)
			{
				for(int c = 0; c < C; ++c)
				{
					Vector3 center = GetSquareWorldPosition(r, c);
					if(_grid[r, c] > 1) Gizmos.color = Color.red;
					else Gizmos.color = Color.white;
					Gizmos.DrawWireCube(center, Vector3.one * SquareSize);
				}
			}
            var mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            var sq = GetSquareAtWorldPosition(mousePos);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(GetSquareWorldPosition(sq.r, sq.c), 0.1f);
		}
#endif
	}
}
