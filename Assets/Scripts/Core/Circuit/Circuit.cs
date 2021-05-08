using UnityEngine;
using System.Collections.Generic;
// using static Game.Helper;
using Game.DSA;
using System.Linq;

/// TODO Create and solve Matrix Equations.
/// TODO Matrix can be filled incrementally instead of rebuilding everytime. But many edge cases.
/// TODO Merge two circuits.
namespace Game.Circuit
{
	/*
	 * Each terminal has an id assigned by the circuit.
	 * The id represents the terminal in UnionFind.
	 * Terminal can ask for its node from its circuit.
	 * New terminals need to unground before connecting to another terminal.
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
			_terminals.Add(ground);
			ground.id = 0;
			_unionFind = new UnionFind(1);
		}

		public int GetNode(Terminal terminal)
		{
			if(!_terminals.Contains(terminal))
			{
				Debug.LogError($"GetNode({terminal.GetInstanceID()}): Circuit does not contain terminal.");
				return -1;
			}
			return _unionFind.FindSetCompressed(terminal.id);
		}

		// Returns list of all nodes that terminal cannot connect to in this circuit.
		public IEnumerable<Terminal> GetNotConnectableTerminals(Terminal terminal)
		{
			// return a list of terminals from this circuit with which terminal can connect
			if(terminal.circuit != this)
			{
				// it can connect to any
				return Enumerable.Empty<Terminal>();
			} else 
			{
				// A terminal cannot connect to terminals sharing same node.
				return _terminals.Where(term => term.Node == terminal.Node);
			}
		}

		/// When a newly created terminal is connected to another terminal. We need to unground it first.
		/// Doing a single reset works because the first edge connected to a new ground terminal ungrounds it.
		public void Unground(Terminal terminal)
		{
			if(terminal.ground) _unionFind.Reset(terminal.id);
			terminal.ground = false;
		}

		/// Assumes that this edge can be added. Does not check for errors.
		public void AddEdge(IEdge edge)
		{
			bool ca = AddTerminal(edge.To);
			bool cb = AddTerminal(edge.From);
			if(ca) edge.To.id = _unionFind.Add();
			if(cb) edge.From.id = _unionFind.Add();

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
