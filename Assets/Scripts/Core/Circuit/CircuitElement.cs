using UnityEngine;

namespace Game.Circuit
{
	public abstract class CircuitElement : MonoBehaviour
	{
		internal int id; // set by circuit after analysing the circuit
		internal bool visited = false;
		public Terminal[] terminals = new Terminal[2];
	}
}
