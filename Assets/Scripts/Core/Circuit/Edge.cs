using System.Collections.Generic;

namespace Game.Circuit
{
    public interface IEdge
	{
		Terminal From { get; set; }
		Terminal To { get; set; }
		int Id { get; set; }
		float Current { get; set; }
	}

    public class IEdgeComparer : IEqualityComparer<IEdge>
    {
        public bool Equals(IEdge x, IEdge y)
        {
			return (x.From == y.From && x.To == y.To) || (x.From == y.To && x.To == y.From);
        }

        public int GetHashCode(IEdge edge)
        {
			int h1 = edge.From.GetHashCode();
			int h2 = edge.To.GetHashCode();
			if(h1 > h2) Game.Helper.Swap(ref h1, ref h2);
			return 31 * h1 + 17 * h2;
        }
    }
}
