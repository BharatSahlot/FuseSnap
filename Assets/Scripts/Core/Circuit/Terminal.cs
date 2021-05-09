using UnityEngine;
using UnityEditor;

namespace Game.Circuit
{
    public class Terminal : MonoBehaviour
	{
		public bool ground = false;
		public bool isComponentTerminal = false;

		private IEdge _component = null;
		public IEdge Component => _component ??= GetComponentInParent<IEdge>();
		internal int id = 0;

		public Circuit circuit;
		public int Node => circuit == null ? -1 : circuit.GetNode(this);

		public float Voltage => circuit != null && circuit.Solved ? circuit.GetTerminalVoltage(this) : 0f;

		private SpriteRenderer _sprite;
        private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
		}

		public void Highlight(bool highlight)
		{
			if(highlight) _sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			else _sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			Handles.Label(transform.position, $"{Node}: {Voltage}", style);
		}
#endif
    }
}
