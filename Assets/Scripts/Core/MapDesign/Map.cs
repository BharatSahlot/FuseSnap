using System.Collections.Generic;

namespace Game.MapDesign
{
    [System.Serializable]
    public class Map
    {
        public int R, C;
        public List<CircuitComponent> Components;
        public List<Wire> Wires;
    }
}
