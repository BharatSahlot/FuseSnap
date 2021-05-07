using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace Game.Circuit
{
	public class Solver
	{
		public List<Terminal> grounds = new List<Terminal>();

		/// Use ufds to assign nodes.
		private (int nodes, int vSources, List<Terminal> terminals) AssignNodes(Terminal ground)
		{
			var terminals = new List<Terminal>();
			var q = new Queue<Terminal>();
			q.Enqueue(ground);

			// Do a bfs to assign a uncompressed node to each terminal
			int tn = 1, vSources = 0;
			ground.visited = true;
			while(q.Count > 0)
			{
				var terminal = q.Dequeue();
				terminals.Add(terminal);
				foreach(var edge in terminal.edgelist)
				{
					if(edge is Battery battery && battery.id == -1)
					{
						battery.id = vSources++;
					}
					
					var to = edge.To == terminal ? edge.From : edge.To;
					if(to.visited) continue;
					to.node = tn++;
					to.visited = true;
					q.Enqueue(to);
				}
			}
			DSA.UnionFind uf = new DSA.UnionFind(tn);
			foreach(Terminal t in terminals) 
			{
				t.visited = false;
				if(t.ground) uf.UnionSet(0, t.node); // all grounds should be same node
			}

			q.Enqueue(ground);
			ground.visited = true;
			// do a bfs and fix the nodes
			while(q.Count > 0)
			{
				var terminal = q.Dequeue();
				foreach(var edge in terminal.edgelist)
				{
					var to = edge.To == terminal ? edge.From : edge.To;

					// if edge is wire, they are same nodes
					if(edge is Wire) uf.UnionSet(edge.From.node, edge.To.node);
					
					if(to.visited) continue;
					to.visited = true;
					q.Enqueue(to);
				}
			}
			
			// Assign nodes and the max node is the node count
			int nodes = 0;
			List<int> par = uf.Compress();
			foreach(Terminal t in terminals)
			{
				t.node = par[t.node];
				if(t.node > nodes) nodes = t.node;
			}
			return (nodes, vSources, terminals);
		}

		public Vector<float> FillMatrix(int nodes, int vSources, List<Terminal> terminals)
		{
			if(nodes <= 1) return Vector<float>.Build.Dense(0);

			Matrix<float> A = Matrix<float>.Build.Dense(nodes + vSources, nodes + vSources);
			Vector<float> Z = Vector<float>.Build.Dense(nodes + vSources);
		
			void StampConductance(int r, int c, float val) => A[r - 1, c - 1] += val;
			void StampVoltage(int r, int c, int val) => A[r - 1, nodes + c] = A[nodes + c, r - 1] = val; 
			void StampVCurrent(int id, float voltage) => Z[nodes + id] = voltage;

			foreach(Terminal u in terminals)
			{
				foreach(var edge in u.edgelist)
				{
				//	if(edge is Wire) continue;

					var v = edge.To == u ? edge.From : edge.To;
					if(v.visited) continue;
				
					if(edge is Battery battery)
					{
						if(battery.From.node != 0) StampVoltage(battery.From.node, battery.id, 1);
						if(battery.To.node != 0)   StampVoltage(battery.To.node, battery.id, -1);
						StampVCurrent(battery.id, battery.voltage);
					} else if(edge is Fuse fuse)
					{
						if(u.node != 0) StampConductance(u.node, u.node, 1.0f / fuse.resistance);
						if(v.node != 0) StampConductance(v.node, v.node, 1.0f / fuse.resistance);
						if(u.node != 0 && v.node != 0)
						{
							StampConductance(u.node, v.node, -1.0f / fuse.resistance);
							StampConductance(v.node, u.node, -1.0f / fuse.resistance);
						}
					}
				}
				u.visited = true;
			}
			return A.Solve(Z);
		}

		private void Update(List<Terminal> terminals, Vector<float> x)
		{
			if(x.Count == 0) return;
			foreach(Terminal t in terminals)
			{
				if(t.GetInstanceID() == -1410) Debug.Log("Updated voltage");
				if(t.node == 0) t.voltage = 0;
				else t.voltage = x[t.node - 1];
			}
		}

		public void Solve()
		{
			var terms = new List<Terminal>();
			foreach(Terminal ground in grounds)
			{
				// this ground was part of another circuit so skip it
				if(ground.visited) continue;
				
				(int nodes, int vSources, List<Terminal> terminals) = AssignNodes(ground);
				Debug.Log($"Found {nodes} nodes and {vSources} batteries.");
				terminals.ForEach(t => t.visited = false);
				Vector<float> x = FillMatrix(nodes, vSources, terminals);
				Update(terminals, x);
				terms.AddRange(terminals);
			}
			// find a cleaner way to do this
			foreach(var t in terms)
			{
				t.visited = false;
				foreach(var e in t.edgelist)
				{
					if(e is Battery battery) battery.id = -1;
				}
			}
		}
	}
}
