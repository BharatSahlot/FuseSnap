namespace Game.Circuit
{
	public interface IEdge
	{
		Terminal From { get; }
		Terminal To { get; }
		int Id { get; set; }
	}
}
