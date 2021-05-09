using MathNet.Numerics.LinearAlgebra;

namespace Game.Circuit
{
    public static class Solver
	{
		public static Vector<float> Solve(Circuit circuit)
		{
			int nodes = circuit.Nodes;
			int vSources = circuit.VSources;
			if(nodes < 1) return null;

			Matrix<float> A = Matrix<float>.Build.Dense(nodes + vSources, nodes + vSources);
			Vector<float> Z = Vector<float>.Build.Dense(nodes + vSources);
			void StampConductance(int r, int c, float val) => A[r - 1, c - 1] += val;
			void StampVoltage(int r, int c, int val) => A[r - 1, nodes + c] = A[nodes + c, r - 1] = val;
			void StampVCurrent(int id, float voltage) => Z[nodes + id] = voltage;

			foreach(IEdge edge in circuit.EdgeList)
			{
				Terminal u = edge.From;
				Terminal v = edge.To;
				if(edge is Battery battery)
				{
					if(!Circuit.IsGround(u)) StampVoltage(u.Node, battery.id, 1);
					if(!Circuit.IsGround(v)) StampVoltage(v.Node, battery.id, -1);
					StampVCurrent(battery.id, battery.voltage);
				} else if(edge is Fuse fuse)
				{
					if(!Circuit.IsGround(u)) StampConductance(u.Node, u.Node, 1.0f / fuse.resistance);
					if(!Circuit.IsGround(v)) StampConductance(v.Node, v.Node, 1.0f / fuse.resistance);
					if(!Circuit.IsGround(u) && !Circuit.IsGround(v))
					{
						StampConductance(u.Node, v.Node, -1.0f / fuse.resistance);
						StampConductance(v.Node, u.Node, -1.0f / fuse.resistance);
					}
				}
			}
			return A.Solve(Z);
		}
	}
}
