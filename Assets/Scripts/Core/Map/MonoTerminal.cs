using UnityEngine;
using UnityEditor;
using System;
using Game.Circuit;

namespace Game.Map
{
    public class MonoTerminal : MonoBehaviour
	{
        public int R { get; set; }
        public int C { get; set; }
        public int P { get; set; } // 1 => top part of cell, -1 => bottom part of cell

        public Terminal Terminal { get; set; }

		private SpriteRenderer _sprite;
        public Action onDestroyed;

        private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
		}

		public void Highlight(bool highlight)
		{
			if(highlight) _sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			else _sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		}

        public void OnDestroy()
        {
            onDestroyed.Invoke();
        }

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			Handles.Label(transform.position, $"{Terminal.Node}: {Terminal.Voltage}", style);
		}
#endif
    }
}
