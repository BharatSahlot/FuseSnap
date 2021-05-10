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

		private int AssignNodes()
		{
			var uf = new Game.DSA.UnionFind(_terminals.Count);
			int[] edgeCount = new int[_terminals.Count];
			foreach(IEdge edge in _edgeList)
			{
				edgeCount[edge.To.id]++;
				edgeCount[edge.From.id]++;
				if(edge is Wire) uf.UnionSet(edge.To.id, edge.From.id);
			}

			// Terminals connected to only one edge are considered grounds.
			foreach(Terminal terminal in _terminals)
			{
				if(edgeCount[terminal.id] == 1) uf.UnionSet(0, terminal.id);
				if(terminal.ground) uf.UnionSet(0, terminal.id); // there can be some designer placed grounds in the circuit
			}

			foreach(Terminal terminal in _terminals) terminal.Node = uf.FindSetCompressed(terminal.id);
			return uf.Components - 1;
		}

		public void Update()
		{
			int nodes = AssignNodes();
			Vector<float> x = Solver.Solve(nodes, _vSources, _edgeList);
			foreach(Terminal terminal in _terminals) terminal.Voltage = terminal.Node == 0 ? 0 : x[terminal.Node - 1];
			foreach(Battery battery in _edgeList.Where(edge => edge is Battery)) battery.Current = x[nodes + battery.Id];
		}

    }
}
