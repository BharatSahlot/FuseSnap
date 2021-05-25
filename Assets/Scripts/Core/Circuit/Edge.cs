using System.Collections.Generic;

namespace Game.Circuit
{
    public class Terminal
    {
        public int Id { get; internal set; }
        public List<Edge> Edges { get; set; }
    }

    public class Edge
    {
        public int Id { get; internal set; }
        public (Terminal, Terminal) Connection { get; set; }
        public float Current { get; private set; }

        public virtual void SetCurrent(float current) => Current = current;
    }

    public interface IEdge
	{
		Terminal From { get; set; }
		Terminal To { get; set; }
		int Id { get; set; }
		float Current { get; set; }
    }

    // Use this class to create a non gui light weight edge.
    // Useful for cases like testing if two terminals can be connected by a new edge.
    public class IEdgeSubsititute : IEdge
    {
        public Terminal From { get; set; }
        public Terminal To { get; set; }
        public int Id { get; set; }
        public float Current { get; set; }
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
