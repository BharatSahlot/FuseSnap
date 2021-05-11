using UnityEngine;

namespace Game.Circuit
{
	// TODO Find how much current is passing through a wire, by searching for connected battery or fuse.
	public class Wire : MonoEdge
	{
		private LineRenderer _line;

		protected override void Awake() 
		{
			_line = GetComponent<LineRenderer>();
		}
		
		protected override void Start()
		{
			_line.positionCount = 2;
			_line.SetPosition(0, To.transform.position);
			_line.SetPosition(1, From.transform.position);
			Graphics.CircuitGrid.Instance.DrawWire(From.transform.position, To.transform.position);
		}

		private void Update()
		{
		}
	}
}
