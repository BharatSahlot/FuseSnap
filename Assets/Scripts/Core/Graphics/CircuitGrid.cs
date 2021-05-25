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
		
        /// Square size in screen space
		public float SquareSize { get; private set; }
        /// Square size in world space
		public float WSquareSize { get; private set; }
		public int R { get; private set; }
		public int C { get; private set; }

		private int[,] _grid;
        private Camera _camera;

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

			_camera = Camera.main;

            SquareSize = Screen.width * _squareToScreenWidthRatio;
            R = Mathf.RoundToInt(Screen.width / SquareSize);
            C = Mathf.RoundToInt(Screen.height / SquareSize);
			_grid = new int[R, C];

            Vector3 size = _camera.ViewportToWorldPoint(new Vector3(1, 1)) - _camera.ViewportToWorldPoint(new Vector3(0, 0));
            WSquareSize = size.x / R;
		}


		private void FillSquare(int r, int c, int cost, int nCost)
		{
			_grid[r, c] += cost;
			if(r + 1 < R) _grid[r + 1, c] += nCost;
			if(r - 1 >= 0) _grid[r - 1, c] += nCost;
			if(c + 1 < C) _grid[r, c + 1] += nCost;
			if(c - 1 >= 0) _grid[r, c - 1] += nCost;
		}

        private void FillSquare(int r, int c) => FillSquare(r, c, _filledSquareCost, _filledNeighbourCost);
        private void UnFillSquare(int r, int c) => FillSquare(r, c, -_filledSquareCost, -_filledNeighbourCost);
		
		public void FillBounds(Bounds worldSpace, Bounds localSpace, Transform obj, int cost, int nCost)
        {
			Vector3 center = worldSpace.center;
			Vector3 extents = worldSpace.extents;

            var (sr, sc) = GetSquareAtWorldPosition(worldSpace.min);
            var (er, ec) = GetSquareAtWorldPosition(worldSpace.max);
            for(int r = sr; r <= er; r++)
            {
                for(int c = sc; c <= ec; ++c)
                {
                    var point = GetSquareWorldPosition(r, c);
                    if(localSpace.Contains(obj.InverseTransformPoint(point)))
                    {
                        FillSquare(r, c, cost, nCost);
                    }
                }
            }
        }

		public void AddComponent(Bounds worldSpace, Bounds localSpace, Transform obj)
		{
            FillBounds(worldSpace, localSpace, obj, _filledSquareCost, _filledNeighbourCost);
		}
		
        public void RemoveComponent(Bounds worldSpace, Bounds localSpace, Transform obj)
		{
            FillBounds(worldSpace, localSpace, obj, -_filledSquareCost, -_filledNeighbourCost);
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

		public (int r, int c) GetSquareAtWorldPosition(Vector3 worldPosition)
		{
            var position = _camera.WorldToScreenPoint(worldPosition);
            position.z = 0;
            int r = Mathf.Clamp((int)(position.x / SquareSize), 0, R - 1);
            int c = Mathf.Clamp((int)(position.y / SquareSize), 0, C - 1);
            return (r, c);
		}

		public Vector3 GetSquareWorldPosition(int r, int c)
		{
			Vector3 res = new Vector3();
            res.x = (SquareSize / 2.0f) + r * SquareSize;
            res.y = (SquareSize / 2.0f) + c * SquareSize;
			res = _camera.ScreenToWorldPoint(res);
            res.z = 0;
            return res;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if(_camera == null) return;
			for(int r = 0; r < R; ++r)
			{
				for(int c = 0; c < C; ++c)
				{
					Vector3 center = GetSquareWorldPosition(r, c);
					if(_grid[r, c] > 1) Gizmos.color = Color.red;
					else Gizmos.color = Color.white;
					Gizmos.DrawWireCube(center, Vector3.one * WSquareSize);
				}
			}
            var mousePos = Input.mousePosition;
            mousePos = _camera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            var sq = GetSquareAtWorldPosition(mousePos);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(GetSquareWorldPosition(sq.r, sq.c), 0.1f);
		}
#endif
	}
}
