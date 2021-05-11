using UnityEngine;

namespace Game.Graphics 
{
	public class CircuitGrid : MonoBehaviour
	{
		private static CircuitGrid _instance = null;
		public static CircuitGrid Instance => _instance;

		[SerializeField] private float _squareToScreenWidthRatio = 0.01f;
		[SerializeField] private int _filledSquareCost = 2; // cost of putting wire on top of a filled square
		[SerializeField] private LayerMask _circuitLayerMask;
		
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

		public void AddComponent(Bounds component)
		{
			Vector3 center = component.center;
			Vector3 extents = component.extents;
			for(float x = center.x - extents.x; x <= center.x + extents.x; x += SquareSize)
			{
				for(float y = center.y - extents.y; y <= center.y + extents.y; y += SquareSize)
				{
					var (r, c) = GetSquareAtWorldPosition(new Vector3(x, y, 0));
					_grid[r, c] += _filledSquareCost;
				}
			}
		}

		public void DrawWire(Vector3 a, Vector3 b)
		{
			var start = GetSquareAtWorldPosition(a);
			var end = GetSquareAtWorldPosition(b);
			// Find shortest path between a and b
			var path = DSA.GridShortestPath.GetShortestPath(_grid, start, end);
			foreach((int r, int c) in path) _grid[r, c] += _filledSquareCost;
		}

		// this is working
		public (int r, int c) GetSquareAtWorldPosition(Vector3 worldPosition)
		{
			worldPosition.x += (R * SquareSize) / 2.0f;
			worldPosition.y += (C * SquareSize) / 2.0f;
			int r = (int)(worldPosition.x / SquareSize); 
			int c = (int)(worldPosition.y / SquareSize); 
			return (r, c);
		}

		// this is working
		public Vector3 GetSquareWorldPosition(int r, int c)
		{
			Vector3 res = new Vector3();
			// square world position = center + row * size - something because worldposition 0,0 is at center of screen
			res.x = (SquareSize / 2) + r * SquareSize - (R * SquareSize / 2.0f);
			res.y = (SquareSize / 2) + c * SquareSize - (C * SquareSize / 2.0f);
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
					if(_grid[r, c] != 0) Gizmos.color = Color.red;
					else Gizmos.color = Color.white;
					Gizmos.DrawWireCube(center, Vector3.one * SquareSize);
				}
			}
		}
#endif
	}
}
