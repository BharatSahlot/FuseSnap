using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using UnityEngine;

namespace Game.Circuit
{
    // Solve every circuit together. Should be fine since it will be a sparse matrix, circuits wont be too big. +Saves a lot of code complexity.
    public class Circuit
	{
		private HashSet<Terminal> _terminals = new HashSet<Terminal>();
		private HashSet<IEdge> _edgeList = new HashSet<IEdge>();
		private int _vSources;

		public void AddEdge(IEdge edge)
		{
			// dont add if edge already present
			// ERROR fix hash code function so that edges where only from and to are interchanged are considered same
			if(!_edgeList.Add(edge)) return;

			AddTerminal(edge.To);
			AddTerminal(edge.From);
			if(edge is Battery) edge.Id = _vSources++;
		}

		private void AddTerminal(Terminal terminal)
		{
			if(_terminals.Add(terminal))
			{
				terminal.id = _terminals.Count - 1;
			}
		}
        public bool HasTerminal(Terminal term)
        {
			return _terminals.Contains(term);
        }

		// FIXME Voltage source loop when connecting two wires connected to ground to a battery
		private (int nodes, int wires) AssignNodes()
		{
			var uf = new Game.DSA.UnionFind(_terminals.Count);
			List<Wire>[] wires = new List<Wire>[_terminals.Count];

			int[] edgeCount = new int[_terminals.Count];
			foreach(IEdge edge in _edgeList)
			{
				edgeCount[edge.To.id]++;
				edgeCount[edge.From.id]++;
				// if(edge is Wire) uf.UnionSet(edge.To.id, edge.From.id);
				if(edge is Wire)
				{
					if(wires[edge.To.id] == null) wires[edge.To.id] = new List<Wire>();
					if(wires[edge.From.id] == null) wires[edge.From.id] = new List<Wire>();

					// FIXED Two wires getting out of a single component terminals are considered 1
					if(edge.To.Component == null) wires[edge.To.id].Add(edge as Wire);
					if(edge.From.Component == null) wires[edge.From.id].Add(edge as Wire);
				}
			}
	
			int wc = 0;
			foreach(Wire wire in _edgeList.Where(edge => edge is Wire)) wire.Id = wc++;
			var wireUf = new Game.DSA.UnionFind(wc);
			foreach(List<Wire> list in wires)
			{
				if(list == null) continue;
				if(list.Count == 2)
				{
					wireUf.UnionSet(list[0].Id, list[1].Id);
					// union set any one of the uncommon terminal to the common terminal
					uf.UnionSet(list[0].To.id, list[0].From.id);
				}
			}
			foreach(Wire wire in _edgeList.Where(edge => edge is Wire)) wire.Id = wireUf.FindSetCompressed(wire.Id);

			// Terminals connected to only one edge are considered grounds.
			foreach(Terminal terminal in _terminals)
			{
				if(edgeCount[terminal.id] == 1) uf.UnionSet(0, terminal.id);
				if(terminal.ground) uf.UnionSet(0, terminal.id); // there can be some designer placed grounds in the circuit
			}

			foreach(Terminal terminal in _terminals) terminal.Node = uf.FindSetCompressed(terminal.id);
			return (uf.Components - 1, wireUf.Components);
		}

		public void Update()
		{
			(int nodes, int wires) = AssignNodes();
			Vector<float> x = Solver.Solve(nodes, wires, _vSources, _edgeList);
			foreach(Terminal terminal in _terminals) terminal.Voltage = terminal.Node == 0 ? 0 : x[terminal.Node - 1];
			foreach(IEdge edge in _edgeList)
			{
				if(edge is Battery) edge.Current = x[nodes + edge.Id];
				else if(edge is Fuse fuse) edge.Current = (edge.To.Voltage - edge.From.Voltage) / fuse.resistance;
				else if(edge is Wire) edge.Current = x[nodes + _vSources + edge.Id];
			}
		}
    }
}
