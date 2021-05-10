using UnityEngine;
using System.Collections.Generic;
using Game.DSA;
using MathNet.Numerics.LinearAlgebra;

/// TODO Merge two circuits.
namespace Game.Circuit
{
    /*
	 * Each terminal has an id assigned by the circuit.
	 * The id represents the terminal in UnionFind.
	 * Terminal can ask for its node from its circuit.
	 * Terminals whose component size in UFDS is 1 are also considered grounds.
	 */
    // public class Circuit
	// {
		// public readonly Terminal ground;
		// private List<IEdge> _edgelist;
		// private HashSet<Terminal> _terminals;
		// private HashSet<Terminal> _fakeGrounds;

		// private UnionFind _unionFind;
		// private Vector<float> _x;
		// private bool _dirty = false;
       
		// public IEnumerable<IEdge> EdgeList => _edgelist;
		// public IEnumerable<Terminal> Terminals => _terminals;
		// public int Nodes => _unionFind.Components;
		// public int VSources { get; private set; }

		// public bool Solved => _x != null;
		// public float GetTerminalVoltage(Terminal terminal) => !IsGround(terminal) && Solved ? _x[terminal.Node - 1] : 0;
		// public float GetCurrent(Battery battery) => Solved ? _x[battery.id] : 0;

		// public static bool IsGround(Terminal terminal) => terminal.Node == 0;

		// public Circuit(Terminal ground)
    //     {
    //         this.ground = ground;
    //     	_edgelist = new List<IEdge>();
			// _terminals = new HashSet<Terminal>();
			// _terminals.Add(ground);
			// _fakeGrounds = new HashSet<Terminal>();

			// ground.id = 0;
			// ground.circuit = this;
			// _unionFind = new UnionFind(1);
		// }

		// public int GetNode(Terminal terminal)
		// {
			// if(!_terminals.Contains(terminal))
			// {
				// Debug.LogError($"GetNode({terminal.GetInstanceID()}): Circuit does not contain terminal.");
				// return -1;
			// }
			// return _unionFind.FindSetCompressed(terminal.id);
		// }

		// private void Unground(Terminal terminal)
		// {
			// _unionFind.Reset(terminal.id);
			// _fakeGrounds.Remove(terminal);
			// _dirty = true;
		// }

		// /// Assumes that this edge can be added. Does not check for errors.
		// public void AddEdge(IEdge edge)
		// {
			// if(_fakeGrounds.Contains(edge.To)) Unground(edge.To);
			// if(_fakeGrounds.Contains(edge.From)) Unground(edge.From);

			// bool ca = AddTerminal(edge.To);
			// bool cb = AddTerminal(edge.From);
			// if(ca) 
			// {
				// edge.To.id = _unionFind.Add();
				// if(!(edge is Wire) || (edge is Wire && !edge.To.isComponentTerminal)) _fakeGrounds.Add(edge.To);
				// _unionFind.UnionSet(edge.To.id, 0);
			// }
			// if(cb)
			// {
				// edge.From.id = _unionFind.Add();
				// if(!(edge is Wire) || (edge is Wire && !edge.From.isComponentTerminal)) _fakeGrounds.Add(edge.From);
				// _unionFind.UnionSet(edge.From.id, 0);
			// }

			// if(edge is Wire)
			// {
				// _unionFind.UnionSet(edge.To.id, edge.From.id);
			// } else if(edge is Battery battery) battery.id = VSources++;
			// _edgelist.Add(edge);

			// edge.Circuit = this;
			// _dirty = true;
		// }

		// private bool AddTerminal(Terminal terminal)
		// {
			// terminal.circuit = this;
			// return _terminals.Add(terminal);
		// }

    //     public void Solve()
    //     {
			// if(_dirty)
			// {
				// _x = Solver.Solve(this);
    //     		_dirty = false;
			// }
		// }

		// public static Circuit Merge(Circuit A, Circuit B)
		// {
			// Circuit circuit = new Circuit(A.ground);
			// foreach(IEdge edge in A._edgelist)
			// {
				// circuit.AddEdge(edge);
			// }
			// foreach(IEdge edge in B._edgelist)
			// {
				// IEdge edge1 = edge;
				// if(IsGround(edge.To)) edge.To = A.ground;
				// if(IsGround(edge.From)) edge.From = A.ground;
				// circuit.AddEdge(edge);
			// }
			// return circuit;
		// }
    // }
}
