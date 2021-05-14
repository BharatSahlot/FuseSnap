using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Circuit
{
    public static class Solver
	{
		public static Vector<float> Solve(int nodes, int wires, int vSources, IEnumerable<IEdge> edgeList)
		{
			if(nodes < 1) return null;

			Matrix<float> A = Matrix<float>.Build.Sparse(nodes + wires + vSources, nodes + wires + vSources);
			Vector<float> Z = Vector<float>.Build.Sparse(nodes + wires + vSources);
			void StampConductance(int r, int c, float val) => A[r - 1, c - 1] += val;
			void StampVoltage(int r, int c, int val) => A[r - 1, nodes + c] = A[nodes + c, r - 1] = val;
			void StampVCurrent(int id, float voltage) => Z[nodes + id] = voltage;

			foreach(IEdge edge in edgeList)
			{
				Terminal u = edge.From;
				Terminal v = edge.To;
				if(edge is Battery battery)
				{
					if(u.Node != 0) StampVoltage(u.Node, battery.Id, 1);
					if(v.Node != 0) StampVoltage(v.Node, battery.Id, -1);
					StampVCurrent(battery.Id, battery.voltage);
				} else if(edge is Fuse fuse)
				{
					if(u.Node != 0) StampConductance(u.Node, u.Node, 1.0f / fuse.resistance);
					if(v.Node != 0) StampConductance(v.Node, v.Node, 1.0f / fuse.resistance);
					if(u.Node != 0 && v.Node != 0)
					{
						StampConductance(u.Node, v.Node, -1.0f / fuse.resistance);
						StampConductance(v.Node, u.Node, -1.0f / fuse.resistance);
					}
				} else if(edge is Wire)
				{
					if(edge.To.Node == edge.From.Node) continue;

					// stamp as 0 voltage battery
					// FIXME for battery from is positive and to is negative, but same is not true for wires
					if(u.Node != 0) StampVoltage(u.Node, vSources + edge.Id, 1);
					if(v.Node != 0) StampVoltage(v.Node, vSources + edge.Id, -1);
					StampVCurrent(vSources + edge.Id, 0);
				}
			}
			return A.Solve(Z);
		}
	}
}
