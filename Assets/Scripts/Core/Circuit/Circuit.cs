using UnityEngine;
using System.Collections.Generic;
// using static Game.Helper;
using Game.DSA;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System;

/// TODO Merge two circuits.
namespace Game.Circuit
{
	public static class CircuitMerger
	{
		// How to handle merging of two grounds?
		// 1. When merging two circuits, replace one ground with a fakeground which basically connects everything to the other ground.
		// 2. Players cant really join a ground to some other terminal. So no need to create a fake ground. This fixes it.
		/// Merge Circuit B into Circuit A
		public static Circuit Merge(Circuit A, Circuit B)
		{
			return A;
		}
	}
    /*
	 * Each terminal has an id assigned by the circuit.
	 * The id represents the terminal in UnionFind.
	 * Terminal can ask for its node from its circuit.
	 * Terminals whose component size in UFDS is 1 are also considered grounds.
	 */
    public class Circuit
	{
		public readonly Terminal ground;
		private List<IEdge> _edgelist;
		private HashSet<Terminal> _terminals;
		private UnionFind _unionFind;
		private Vector<float> _x;
		private bool _dirty = false;
       
		public IEnumerable<IEdge> EdgeList => _edgelist;
		public IEnumerable<Terminal> Terminals => _terminals;
		public int Nodes => _unionFind.ComponentsG1;
		public int VSources { get; private set; }

		public bool Solved => _x != null;
		public float GetTerminalVoltage(Terminal terminal) => !IsGround(terminal) && Solved ? _x[terminal.Node - 1] : 0;
		public float GetCurrent(Battery battery) => Solved ? _x[battery.id] : 0;

		public static bool IsGround(Terminal terminal) => terminal.Node == 0;

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
			return _unionFind.GetComponentSize(terminal.id) == 1 ? 0 : _unionFind.FindSetCompressed(terminal.id);
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
			} else if(edge is Battery battery) battery.id = VSources++;
			_edgelist.Add(edge);

			edge.Circuit = this;
			_dirty = true;
		}

		private bool AddTerminal(Terminal terminal)
		{
			terminal.circuit = this;
			return _terminals.Add(terminal);
		}

        public void Solve()
        {
			if(_dirty)
			{
				_x = Solver.Solve(this);
        		_dirty = false;
			}
		}
    }
}
