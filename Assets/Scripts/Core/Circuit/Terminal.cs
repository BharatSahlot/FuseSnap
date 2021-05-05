using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.Circuit
{
	public class Terminal : MonoBehaviour
	{
		public bool ground = false;
		
		internal bool visited = false;
		internal int node = 0;

		public List<IEdge> edgelist = new List<IEdge>();
		[HideInInspector] public float voltage = 0;

		private SpriteRenderer _sprite;

		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
		}

		public void AddEdge(IEdge edge)
		{
			edgelist.Add(edge);
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
			Handles.Label(transform.position, $"{node}: {voltage}", style);
		}
#endif
	}
}
