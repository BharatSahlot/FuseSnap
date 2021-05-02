using UnityEngine;
using System.Collections.Generic;

namespace Game.Circuit
{
	public class Terminal 
	{
		public int node;
		public bool visited;

		public Vector3 position;
		public List<Terminal> connections;
		public readonly CircuitElement parent;
	
		public Terminal next;	

		public Terminal(Vector3 pos, CircuitElement par, Terminal nxt)
		{
			position = pos;
			parent   = par;
			next     = nxt;
		}
	}
}
