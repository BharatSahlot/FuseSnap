using UnityEngine;

namespace Game.Circuit
{
	public class Fuse : CircuitElement
	{
		public float resistance;
		public float max_current; // more current than this and the fuse will break
	}
}
