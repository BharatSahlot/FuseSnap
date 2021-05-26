using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Circuit
{
    public static class Solver
	{
		public static Vector<float> Solve(int nodes, int vSources, IEnumerable<Edge> edgeList)
		{
			if(nodes < 1) return null;

			Matrix<float> A = Matrix<float>.Build.Sparse(nodes + vSources, nodes + vSources);
			Vector<float> Z = Vector<float>.Build.Sparse(nodes + vSources);
			void StampConductance(int r, int c, float val) => A[r - 1, c - 1] += val;
			void StampVoltage(int r, int c, int val) => A[r - 1, nodes + c] = A[nodes + c, r - 1] = val;
			void StampVCurrent(int id, float voltage) => Z[nodes + id] = voltage;

			foreach(Edge edge in edgeList)
			{
				Terminal u = edge[0];
				Terminal v = edge[1];
				if(edge is Battery battery)
				{
					if(u.Node != 0) StampVoltage(u.Node, battery.Id, 1);
					if(v.Node != 0) StampVoltage(v.Node, battery.Id, -1);
					StampVCurrent(battery.Id, battery.Voltage);
				} else if(edge is IResistor resistor)
				{
					if(u.Node != 0) StampConductance(u.Node, u.Node, 1.0f / resistor.Resistance);
					if(v.Node != 0) StampConductance(v.Node, v.Node, 1.0f / resistor.Resistance);
					if(u.Node != 0 && v.Node != 0)
					{
						StampConductance(u.Node, v.Node, -1.0f / resistor.Resistance);
						StampConductance(v.Node, u.Node, -1.0f / resistor.Resistance);
					}
				}
			}
			return A.Solve(Z);
		}
	}
}
