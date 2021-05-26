using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using UnityEngine;

namespace Game.Circuit
{
    public class Circuit
	{
		private HashSet<Terminal> _terminals = new HashSet<Terminal>();
		private HashSet<Edge> _edgeList = new HashSet<Edge>();
        public Terminal Ground { get; private set; }

        public bool HasTerminal(Terminal term) => _terminals.Contains(term);

        public Circuit(Terminal ground)
        {
            Ground = ground;
            AddTerminal(ground);
        }

        public bool CanAddEdge(Terminal a, Terminal b)
        {
            var temp = new Connection(a, b);
            return !_edgeList.Contains(temp);
        }

        public bool AddEdge(Edge edge)
		{
			// dont add if edge already present
			if(!_edgeList.Add(edge)) return false;
            foreach(var t in edge)
            {
                AddTerminal(t);
                t.Edges.Add(edge);
                if(t.Component != null && t.Component != edge) AddEdge(t.Component);
            }
			return true;
		}

        private void AddTerminal(Terminal terminal)
		{
			if(_terminals.Add(terminal))
			{
				terminal.Id = _terminals.Count - 1;
			}
		}

        private void Regen()
        {
            int id = 0;
            foreach(Terminal terminal in _terminals)
            {
                terminal.Id = id++;
            }
        }

        public void Solve()
        {
            Regen();

            (int nodes, int vSources) = AssignNodes();
            Vector<float> x = Solver.Solve(nodes, vSources, _edgeList);
			foreach(Terminal terminal in _terminals) terminal.Voltage = terminal.Node == 0 ? 0 : x[terminal.Node - 1];
            
            foreach(Resistor resistor in _edgeList)
            {
                Edge edge = resistor as Edge;
                if(resistor == null) continue;
                if(edge[0].Node != edge[1].Node)
                {
                    float current = (edge[0].Voltage - edge[1].Voltage) / resistor.CombinedResistance;
                    foreach(Resistor res in _edgeList)
                    {
                        if(res != null && res.CombinedId == resistor.CombinedId) 
                        {
                            Edge e = res as Edge;
                            e.SetCurrent(current * e.Direction);
                        }
                    }
                }
            }
            foreach(Battery battery in _edgeList)
            {
                if(battery == null) continue;
                battery.SetCurrent(x[nodes + battery.Id]);
            }
            // onSolve.Invoke();
        }

		private (int nodes, int vSources) AssignNodes()
        {
            var uf = new Game.DSA.UnionFind(_terminals.Count);
            var resistors = new List<Resistor>[_terminals.Count];

            int vSources = 0;
            foreach(Edge edge in _edgeList)
            {
                if(edge is Resistor res)
                {
                    foreach(var t in edge)
                    {
                        if(resistors[t.Id] == null) resistors[t.Id] = new List<Resistor>();
                        if(t.Component is Resistor) resistors[t.Id].Add(res);
                    }
                } else if(edge is Battery) edge.Id = vSources++;
            }

            int wc = 0;
            foreach(Resistor res in _edgeList.Where(res => res is Resistor)) res.CombinedId = wc++;

            var wireUf = new Game.DSA.UnionFind(wc);
            foreach(var list in resistors)
            {
                if (list == null) continue;
                if (list.Count == 2)
                {
                    wireUf.UnionSet(list[0].Id, list[1].Id);
                    // union set any one of the uncommon terminal to the common terminal
                    uf.UnionSet(list[0][0].Id, list[0][1].Id);
                }
            }

            foreach(Resistor res in _edgeList)
            {
                if(res == null) continue;
                res.CombinedId = wireUf.FindSetCompressed(res.CombinedId);
                res.CombinedResistance = wireUf.GetComponentSize(res.CombinedId) * res.CombinedResistance;
            }

            foreach (Terminal terminal in _terminals)
            {
                // Terminals connected to only one edge are considered grounds.
                if (terminal.Edges.Count == 1) uf.UnionSet(0, terminal.Id);
            }
            foreach (Terminal terminal in _terminals) terminal.Node = uf.FindSetCompressed(terminal.Id);
            return (uf.Components - 1, vSources);
        }

        
        /// <summary>
        /// Marks all edges reachable from ground node and returns all the edges not reachable from ground node.
        /// <summary/>
        private IEnumerable<Edge> GetUnreachableEdges(HashSet<Edge> exclude)
        {
            // bfs from ground
            DSA.UnionFind uf = new DSA.UnionFind(_terminals.Count);
            foreach(Edge edge in _edgeList)
            {
                if(exclude.Contains(edge)) continue;

                if(edge is Fuse || edge is Battery) 
                {
                    uf.UnionSet(edge[0].Id, 0);
                    uf.UnionSet(edge[1].Id, 0);
                }
                uf.UnionSet(edge[0].Id, edge[1].Id);
            }
            var edges = new List<Edge>();
            foreach(var edge in _edgeList)
            {
                if(exclude.Contains(edge)) continue;
                if(uf.FindSet(edge[0].Id) != 0 && uf.FindSet(edge[1].Id) != 0)
                {
                    edges.Add(edge);
                }
            }
            return edges;
        }

        private void RemoveEdge(Edge edge)
        {
            _edgeList.Remove(edge);
            foreach(var t in edge)
            {
                t.Edges.Remove(edge);
                if(t.Edges.Count == 0) RemoveTerminal(t);
            }
            // Destroy itself
            edge.OnRemove();
        }

        private void RemoveTerminal(Terminal terminal)
        {
            _terminals.Remove(terminal);
            terminal.OnRemove();
        }

        private void AssignDirections()
        {
            var q = new Stack<Terminal>();
            var visited = new HashSet<Edge>();

            if(Ground.Edges.Count < 1) return;

            Ground.Edges[0].Direction = 1;
            visited.Add(Ground.Edges[0]);
            q.Push(Ground);

            while(q.Count > 0)
            {
                Terminal t = q.Pop();

                Edge r = t.Edges.First(e => visited.Contains(e));
                foreach(Edge e in t.Edges)
                {
                    if(e == r) continue;

                    int dir = 0;
                    if(r[0] == e[1] || r[1] == e[0]) dir = r.Direction;
                    else dir = -r.Direction;

                    if(visited.Contains(e))
                    {
                        if(e.Direction != dir)
                        {
                            Debug.LogError("Cannot assign directions to edges.");
                        }
                    } else 
                    {
                        q.Push(e[0] == t ? e[1] : e[0]);
                        visited.Add(e);
                    }
                }
            }
        }
    }
}
