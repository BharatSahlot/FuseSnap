namespace Game.Circuit
{
    // public class EdgeComparer : IComparer<IEdge>
    // {
    //     public int Compare(IEdge x, IEdge y)
    //     {
			// if((x.From == y.From && x.To == y.To) || (x.From == y.To && x.To == y.From))
			// {
				// return 0;
			// }
			// return x.Id.CompareTo(y.Id);
    //     }
    // }
    public interface IEdge
	{
		Terminal From { get; set; }
		Terminal To { get; set; }
		int Id { get; set; }
	}
}
