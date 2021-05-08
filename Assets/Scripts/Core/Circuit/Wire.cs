using UnityEngine;

namespace Game.Circuit
{
	// TODO Find how much current is passing through a wire, by searching for connected battery or fuse.
	public class Wire : MonoBehaviour, IEdge
	{
		public Terminal From { get; set; }
		public Terminal To { get; set; }
        public int Id { get; set; }
	
		private LineRenderer _line;
		
		private void Awake()
		{
			_line = GetComponent<LineRenderer>();
		}

		public void Init()
		{
			_line.positionCount = 2;
			_line.SetPositions(new Vector3 [] { From.transform.position, To.transform.position });
		}

		private void Update()
		{
		}
	}
}
