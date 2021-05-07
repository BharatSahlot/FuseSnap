using UnityEngine;
using System.Collections.Generic;
using static Game.Helper;
using Game.DSA;

/// TODO Implement a function to check if a new edge can be added.
/// TODO Create and solve Matrix Equations.
/// TODO Matrix can be filled incrementally instead of rebuilding everytime.
/// TODO Merge two circuits.
namespace Game.Circuit
{
	/*
	 * Each terminal has an id assigned by the circuit.
	 * The id represents the terminal in UnionFind.
	 * Terminal can ask for its node from its circuit.
	 */
	public class Circuit
	{
		public readonly Terminal ground;
		private List<IEdge> _edgelist;
		private HashSet<Terminal> _terminals;
		private UnionFind _unionFind;
		private int _nodes;

        public Circuit(Terminal ground)
        {
            this.ground = ground;
        	_edgelist = new List<IEdge>();
			_terminals = new HashSet<Terminal>();
			_unionFind = new UnionFind(1);
		}

		public int GetNode(Terminal terminal)
		{
			if(!_terminals.Contains(terminal))
			{
				Debug.LogError("Terminal connected to circuit, but no edge.");
				return -1;
			}
			return _unionFind.FindSet(terminal.id);
		}

		/// Assumes that this edge does not merge circuits.
		/// Circuit merge should be performed before adding edge, incase needed.
		public void AddEdge(IEdge edge)
		{
			bool ca = AddTerminal(edge.To);
			bool cb = AddTerminal(edge.From);
			if(!ca) edge.To.id = _unionFind.Add();
			if(!cb) edge.From.id = _unionFind.Add();
			if(edge is Wire)
			{
				_unionFind.UnionSet(edge.To.id, edge.From.id);
			}
			_nodes = _unionFind.Components;
		}

		private bool AddTerminal(Terminal terminal)
		{
			terminal.circuit = this;
			return _terminals.Add(terminal);
		}
    }
}
