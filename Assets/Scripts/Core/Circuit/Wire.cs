using UnityEngine;

namespace Game.Circuit
{
	// TODO Find how much current is passing through a wire, by searching for connected battery or fuse.
	public class Wire : MonoEdge
	{
		[SerializeField] private float _currentSpeed = 0.3f;

		internal float Resistance { get; set; }
        public int Direction { get; internal set; }

        private LineRenderer _line;
		private Material _mat;
		private float _flow;
		private Vector2 _defaultScale;

		protected override void Awake() 
		{
			_line = GetComponent<LineRenderer>();
			_mat = _line.material;
			_defaultScale = _mat.GetTextureScale("_CurrentTexture");
		}
		
		protected override void Start()
		{
			// _line.positionCount = 2;
			// _line.SetPosition(0, To.transform.position);
			// _line.SetPosition(1, From.transform.position);
			Graphics.CircuitGrid.Instance.DrawWire(From.transform.position, To.transform.position, _line);
		}

		private void Update()
		{
			_flow += Current * Time.deltaTime * _currentSpeed;
			_mat.SetFloat("_Current", _flow);
			_mat.SetTextureScale("_CurrentTexture", new Vector2(Mathf.Sign(-_flow) * _defaultScale.x, _defaultScale.y));
			if(Current != 0) _mat.SetInt("_IsCurrentFlowing", 1);
			else _mat.SetInt("_IsCurrentFlowing", 0);
		}
	}
}
