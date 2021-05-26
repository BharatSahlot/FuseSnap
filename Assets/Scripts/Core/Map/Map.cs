using System.Collections.Generic;
using MessagePack;

namespace Game.Map
{
    [MessagePackObject]
    public class Map
    {
        [Key(0)]
        public int R { get; set; }
        [Key(1)]
        public int C { get; set; }
        [Key(2)]
        public List<MapEdgeData> Edges;
        // public string Serialize()
        // {
        //     StringBuilder res = new StringBuilder();
        //     res.Append(R); res.Append(':');
        //     res.Append(C); res.Append(':');
        //     res.Append(Edges.Count); res.Append(':');
        //     foreach(MonoEdge edge in Edges)
        //     {
        //         res.Append('\n');
        //         res.Append(edge.R); res.Append(':');
        //         res.Append(edge.C); res.Append(':');
        //         if(edge.Edge is Battery battery)
        //         {
        //             res.Append(edge.Rotation); res.Append(':');
        //             res.Append(":B:");
        //             res.Append(battery.Voltage);
        //         } else if(edge.Edge is Fuse fuse)
        //         {
        //             res.Append(edge.Rotation); res.Append(':');
        //             res.Append(":F:");
        //             res.Append(fuse.Resistance); res.Append(':');
        //             res.Append(fuse.MaxCurrent);
        //         } else if(edge.Edge is Resistor resistor)
        //         {
        //             res.Append(":W:");
        //             res.Append(resistor.Resistance);
        //         }
        //     }
        //     return res.ToString();
        // }

        // public static Map CreateFromString(string s)
        // {
        //     Map map = new Map();
        //     using(StringReader reader = new StringReader(s))
        //     {
        //     }
        //     return map;
        // }
    }
}
