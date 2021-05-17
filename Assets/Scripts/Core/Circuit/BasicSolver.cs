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
		private HashSet<IEdge> _edgeList = new HashSet<IEdge>(new IEdgeComparer());
		private int _vSources;

        public const float wireResistance = 0.01f;

        public bool AddEdge(IEdge edge)
		{
			// dont add if edge already present
			// FIXED fix hash code function so that edges where only from and to are interchanged are considered same
			if(!_edgeList.Add(edge)) return false;

			AddTerminal(edge.To);
			AddTerminal(edge.From);
			if(edge is Battery) edge.Id = _vSources++;
			return true;
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

			HashSet<Wire> visited = new HashSet<Wire>();
			void Bfs(Wire wire)
			{
				if(visited.Contains(wire)) return;

				wire.Direction = 1;
				var st = new Stack<Wire>();
				st.Push(wire);
				while(st.Count > 0)
				{
					wire = st.Pop();
					visited.Add(wire);

					var list = new List<Wire>();
					if(wires[wire.From.id].Count == 2)
					{
						Wire other = wires[wire.From.id][0] == wire ? wires[wire.From.id][1] : wires[wire.From.id][0];
						list.Add(other);
					}
					if(wires[wire.To.id].Count == 2)
					{
						Wire other = wires[wire.To.id][0] == wire ? wires[wire.To.id][1] : wires[wire.To.id][0];
						list.Add(other);
					}
					foreach(Wire other in list)
					{
						int dir = 1;
						if(wire.To == other.To) dir = wire.Direction;
						else dir = wire.Direction * -1;
						if(visited.Contains(other))
						{
							if(other.Direction != dir) Debug.LogError("[Wire Direction Assign Error]: Same wire is assigned different directions.");
						} else 
						{
							other.Direction = dir;
							st.Push(other);
							visited.Add(wire);
						}
					}
				}
			}
			foreach(Wire wire in _edgeList.Where(e => e is Wire)) Bfs(wire);
	
			// if list[0].to == list[1].from then same direction current
			// else opposite direction current

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

			foreach(Wire wire in _edgeList.Where(edge => edge is Wire))
			{
				wire.Id = wireUf.FindSetCompressed(wire.Id);
				wire.Resistance = wireUf.GetComponentSize(wire.Id) * wireResistance;
			}

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
			Vector<float> x = Solver.Solve(nodes, _vSources, _edgeList);
			foreach(Terminal terminal in _terminals) terminal.Voltage = terminal.Node == 0 ? 0 : x[terminal.Node - 1];

			float[] wireCurrent = new float[wires];
			foreach(Wire wire in _edgeList.Where(e => e is Wire wire && wire.To.Node != wire.From.Node))
			{
				wireCurrent[wire.Id] = wire.Direction * (wire.From.Voltage - wire.To.Voltage) / wire.Resistance;
			}
			
			foreach(IEdge edge in _edgeList)
			{
				if(edge is Battery) edge.Current = x[nodes + edge.Id];
				else if(edge is Fuse fuse) edge.Current = (edge.To.Voltage - edge.From.Voltage) / fuse.resistance;
				// FIXME get wire current direction
				else if(edge is Wire wire) wire.Current = wireCurrent[wire.Id] * wire.Direction;
			}
		}
    }
}
