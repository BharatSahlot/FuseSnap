using UnityEngine;
using UnityEditor;
using System;

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

		[HideInInspector] public float voltage = 0;

		private SpriteRenderer _sprite;
        internal bool fakeGround;

        private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
		}

		public void Highlight(bool highlight)
		{
			if(highlight) _sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			else _sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		}
        
		internal void Unground()
        {
			if(circuit == null)
			{
				Debug.LogError("Attempted to unground a terminal not part of a circuit.");
			}
 			circuit.Unground(this);
			fakeGround = false;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			Handles.Label(transform.position, $"{Node}: {voltage}", style);
		}
#endif
    }
}
