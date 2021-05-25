using UnityEngine;

namespace Game.Circuit
{
	public class Wire : MonoEdge
	{
		[SerializeField] private float _currentSpeed = 0.3f;
        [SerializeField] private bool _drawWire = true;

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
			if(_drawWire) Graphics.CircuitGrid.Instance.DrawWire(A.transform.position, B.transform.position, _line);
		}

		private void Update()
		{
            if(!_drawWire) return;

			_flow += Edge.Current * Time.deltaTime * _currentSpeed;
			_mat.SetFloat("_Current", _flow);
			_mat.SetTextureScale("_CurrentTexture", new Vector2(Mathf.Sign(-Edge.Current) * _defaultScale.x, _defaultScale.y));
			if(Edge.Current != 0) _mat.SetInt("_IsCurrentFlowing", 1);
			else _mat.SetInt("_IsCurrentFlowing", 0);
		}
	}
}
