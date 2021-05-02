using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace Game.Circuit
{
	public class Circuit : MonoBehaviour
	{
		public List<CircuitElement> circuitElements;
		Matrix<double> A, B;

		public void Solve()
		{
			var q = new Queue<Terminal>();
			int id = 0;
			foreach(var ele in circuitElements)
			{
				if(ele is Ground)
				{
					ele.id = id++;
					q.Enqueue(ele.terminals[0]);
				}
			}
		
			// Resistors are labelled seperately of Voltages
			// Every element is between two nodes. So each terminal is a node.	
			int nodes = 0;
			while(q.Count > 0)
			{
				var terminal = q.Dequeue();
				foreach(Terminal t in terminal.connections)
				{
					if(t.visited) continue;
					t.node = terminal.node;
					t.visited = true;
				
					if(t.parent is Wire) t.next.node = t.node;
					else t.next.node = nodes++;

					t.next.visited = true;
					q.Enqueue(t.next);
				}
			}

			// TODO: Add some basic logic to draw circuits and test if labeling of nodes is working properly
		}
	}
}
