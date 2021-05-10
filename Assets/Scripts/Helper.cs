using System;
using System.Linq;

namespace Game
{
	public static class Helper
	{
		public static T Max<T>(params T[] values)
		{
			return values.Max();
		}

        public static void Swap<T>(ref T i, ref T j)
    	{
			T temp = i;
			i = j;
			j = temp;
        }
    }
}
