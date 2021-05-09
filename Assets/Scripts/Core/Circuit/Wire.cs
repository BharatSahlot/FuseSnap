using UnityEngine;

namespace Game.Circuit
{
	// TODO Find how much current is passing through a wire, by searching for connected battery or fuse.
	public class Wire : MonoBehaviour, IEdge
	{
		[field: SerializeField] public Terminal From { get; set; }
		[field: SerializeField] public Terminal To { get; set; }
        public int Id { get; set; }
		public Circuit Circuit { get; set; }
	
		private LineRenderer _line;
		
		public void Init()
		{
			if(_line == null) _line = GetComponent<LineRenderer>();
			_line.positionCount = 2;
			_line.SetPositions(new Vector3 [] { From.transform.position, To.transform.position });
		}

		private void Update()
		{
		}
	}
}
