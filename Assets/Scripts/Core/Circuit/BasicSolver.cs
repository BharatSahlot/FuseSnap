using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using UnityEngine;

namespace Game.Circuit
{
    public class Circuit
	{
		private HashSet<Terminal> _terminals = new HashSet<Terminal>();
		private HashSet<IEdge> _edgeList = new HashSet<IEdge>(new IEdgeComparer());
        private List<int> _edgeCount = new List<int>();

        public const float wireResistance = 0.01f;
        public bool HasTerminal(Terminal term) => _terminals.Contains(term);

        public bool CanAddEdge(Terminal a, Terminal b)
        {
            var temp = new IEdgeSubsititute 
            {
                From = a, To = b
            };
            return !_edgeList.Contains(temp);
        }

        public bool AddEdge(IEdge edge)
		{
			// dont add if edge already present
			if(!_edgeList.Add(edge)) return false;

			AddTerminal(edge.To);
			AddTerminal(edge.From);
            _edgeCount[edge.To.id]++;
            _edgeCount[edge.From.id]++;
			return true;
		}

        private void AddTerminal(Terminal terminal)
		{
			if(_terminals.Add(terminal))
			{
				terminal.id = _terminals.Count - 1;
                _edgeCount.Add(0);
			}
		}

        private void Regen()
        {
            int id = 0;
            _edgeCount = new List<int>(_edgeCount.Count);
            foreach(Terminal terminal in _terminals)
            {
                terminal.id = id++;
                _edgeCount.Add(0);
            }

            foreach(IEdge edge in _edgeList)
            {
                _edgeCount[edge.To.id]++;
                _edgeCount[edge.From.id]++;
            }
        }

        public void Update()
		{
			(int nodes, int wires, int vSources) = AssignNodes();
			Vector<float> x = Solver.Solve(nodes, vSources, _edgeList);
			foreach(Terminal terminal in _terminals) terminal.Voltage = terminal.Node == 0 ? 0 : x[terminal.Node - 1];

			float[] wireCurrent = new float[wires];
			foreach(Wire wire in _edgeList.Where(e => e is Wire wire && wire.To.Node != wire.From.Node))
			{
				wireCurrent[wire.Id] = wire.Direction * (wire.From.Voltage - wire.To.Voltage) / wire.Resistance;
			}
			
            HashSet<IEdge> overHeatedFuses = new HashSet<IEdge>();
			foreach(IEdge edge in _edgeList)
			{
				if(edge is Battery) edge.Current = x[nodes + edge.Id];
				else if(edge is Fuse fuse)
                {
                    edge.Current = (edge.To.Voltage - edge.From.Voltage) / fuse.resistance;
                    if(Mathf.Abs(edge.Current) > fuse.max_current) overHeatedFuses.Add(fuse);
                }
                else if(edge is Wire wire) wire.Current = wireCurrent[wire.Id] * wire.Direction;
			}
            // TODO remove any overheated fuse, then do a bfs from the ground and remove any edges/terminals not reachable from the ground
		    var toRemove = GetUnreachableEdges(overHeatedFuses);
            foreach(var fuse in overHeatedFuses) RemoveEdge(fuse); 
            foreach(var edge in toRemove) RemoveEdge(edge);

            // Rebuild every already existing info
            if(overHeatedFuses.Count > 0) Regen();
        }

		private (int nodes, int wires, int vSources) AssignNodes()
        {
            var uf = new Game.DSA.UnionFind(_terminals.Count);
            List<Wire>[] wires = new List<Wire>[_terminals.Count];

            int vSources = 0;
            foreach (IEdge edge in _edgeList)
            {
                if (edge is Wire)
                {
                    if (wires[edge.To.id] == null) wires[edge.To.id] = new List<Wire>();
                    if (wires[edge.From.id] == null) wires[edge.From.id] = new List<Wire>();

                    if (edge.To.Component == null) wires[edge.To.id].Add(edge as Wire);
                    if (edge.From.Component == null) wires[edge.From.id].Add(edge as Wire);
                } else if(edge is Battery) edge.Id = vSources++;
            }

            AssignWireDirections(wires);

            int wc = 0;
            foreach (Wire wire in _edgeList.Where(edge => edge is Wire)) wire.Id = wc++;
            var wireUf = new Game.DSA.UnionFind(wc);
            foreach (List<Wire> list in wires)
            {
                if (list == null) continue;
                if (list.Count == 2)
                {
                    wireUf.UnionSet(list[0].Id, list[1].Id);
                    // union set any one of the uncommon terminal to the common terminal
                    uf.UnionSet(list[0].To.id, list[0].From.id);
                }
            }

            foreach (Wire wire in _edgeList.Where(edge => edge is Wire))
            {
                wire.Id = wireUf.FindSetCompressed(wire.Id);
                wire.Resistance = wireUf.GetComponentSize(wire.Id) * wireResistance;
            }

            foreach (Terminal terminal in _terminals)
            {
                // Terminals connected to only one edge are considered grounds.
                if (_edgeCount[terminal.id] == 1) uf.UnionSet(0, terminal.id);
                if (terminal.ground) uf.UnionSet(0, terminal.id); // there can be some designer placed grounds in the circuit
            }

            foreach (Terminal terminal in _terminals) terminal.Node = uf.FindSetCompressed(terminal.id);
            return (uf.Components - 1, wireUf.Components, vSources);
        }


        
        /// <summary>
        /// Marks all edges reachable from ground node and returns all the edges not reachable from ground node.
        /// <summary/>
        private IEnumerable<IEdge> GetUnreachableEdges(HashSet<IEdge> exclude)
        {
            // bfs from ground
            Terminal ground = _terminals.First(t => t.ground);
            DSA.UnionFind uf = new DSA.UnionFind(_terminals.Count);
            foreach(IEdge edge in _edgeList)
            {
                if(exclude.Contains(edge)) continue;
                if(!(edge is Wire))
                {
                    uf.UnionSet(edge.To.id, 0);
                    uf.UnionSet(edge.From.id, 0);
                }
                uf.UnionSet(edge.To.id, edge.From.id);
            }
            List<IEdge> edges = new List<IEdge>();
            foreach(IEdge edge in _edgeList)
            {
                if(exclude.Contains(edge)) continue;
                if(uf.FindSet(edge.To.id) != 0 && uf.FindSet(edge.From.id) != 0)
                {
                    edges.Add(edge);
                }
            }
            return edges;
        }

        private void RemoveEdge(IEdge edge)
        {
            _edgeCount[edge.To.id]--;
            _edgeCount[edge.From.id]--;
            _edgeList.Remove(edge);

            if(_edgeCount[edge.To.id] == 0) RemoveTerminal(edge.To);
            if(_edgeCount[edge.From.id] == 0) RemoveTerminal(edge.From);

            if(edge is MonoEdge e)
            {
                e.transform.DetachChildren();
                GameObject.Destroy(e.gameObject);
            }
        }

        private void RemoveTerminal(Terminal terminal)
        {
            _terminals.Remove(terminal);
            GameObject.Destroy(terminal.gameObject);
        }

        private void AssignWireDirections(List<Wire>[] wires)
        {
            HashSet<Wire> visited = new HashSet<Wire>();
            void Bfs(Wire wire)
            {
                if (visited.Contains(wire)) return;

                wire.Direction = 1;
                var st = new Stack<Wire>();
                st.Push(wire);
                while (st.Count > 0)
                {
                    wire = st.Pop();
                    visited.Add(wire);

                    var list = new List<Wire>();
                    if (wires[wire.From.id].Count == 2)
                    {
                        Wire other = wires[wire.From.id][0] == wire ? wires[wire.From.id][1] : wires[wire.From.id][0];
                        list.Add(other);
                    }
                    if (wires[wire.To.id].Count == 2)
                    {
                        Wire other = wires[wire.To.id][0] == wire ? wires[wire.To.id][1] : wires[wire.To.id][0];
                        list.Add(other);
                    }
                    foreach (Wire other in list)
                    {
                        int dir = 1;
                        if (wire.To == other.From || wire.From == other.To) dir = wire.Direction;
                        else dir = wire.Direction * -1;
                        if (visited.Contains(other))
                        {
                            if (other.Direction != dir)
                                Debug.LogError("[Wire Direction Assign Error]: Same wire is assigned different directions.");
                        }
                        else
                        {
                            other.Direction = dir;
                            st.Push(other);
                            visited.Add(wire);
                        }
                    }
                }
            }
            foreach (Wire wire in _edgeList.Where(e => e is Wire)) Bfs(wire);
        }
    }
}
