namespace Game.Circuit
{
	public interface IEdge
	{
		Terminal From { get; set; }
		Terminal To { get; set; }
	}
}
