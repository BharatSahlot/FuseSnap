using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace Game.Circuit
{
    public static class Solver
	{
		public static Vector<float> Solve(int nodes, int vSources, IEnumerable<IComponent> components)
		{
			if(nodes < 1) return null;

			Matrix<float> A = Matrix<float>.Build.Sparse(nodes + vSources, nodes + vSources);
			Vector<float> Z = Vector<float>.Build.Sparse(nodes + vSources);
			void StampConductance(int r, int c, float val) => A[r - 1, c - 1] += val;
			void StampVoltage(int r, int c, int val) => A[r - 1, nodes + c] = A[nodes + c, r - 1] = val;
			void StampVCurrent(int id, float voltage) => Z[nodes + id] = voltage;

			foreach(IComponent component in components)
			{
                int u = component.GetNode(1);
                int v = component.GetNode(-1);
				if(component is IVoltageSource source)
				{
					if(u != 0) StampVoltage(u, source.VSourceId, 1);
					if(v != 0) StampVoltage(v, source.VSourceId, -1);
					StampVCurrent(source.VSourceId, source.Voltage);
				} else if(component is IResistor resistor)
				{
					if(u != 0) StampConductance(u, u, 1.0f / resistor.Resistance);
					if(v != 0) StampConductance(v, v, 1.0f / resistor.Resistance);
					if(u != 0 && v != 0)
					{
						StampConductance(u, v, -1.0f / resistor.Resistance);
						StampConductance(v, u, -1.0f / resistor.Resistance);
					}
				}
			}
			return A.Solve(Z);
		}
	}
}
