using System.Linq;

namespace Game
{
	public static class Helper
	{
		public static T Max<T>(params T[] values)
		{
			return values.Max();
		}
	}
}
