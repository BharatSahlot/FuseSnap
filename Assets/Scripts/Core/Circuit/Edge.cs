using System.Collections.Generic;

namespace Game.Circuit
{
    public enum EdgeTypes
    {
        Wire, Battery, Fuse
    }

    [System.Serializable]
    public class Terminal
    {
        public int Id { get; internal set; }
        public int Player { get; set; }
        public int Node { get; internal set; }
        public float Voltage { get; internal set; }
        public Edge Component { get; internal set; }
        public List<Edge> Edges { get; internal set; }
        
        public virtual void OnRemove() {}
    }

    [System.Serializable]
    public class Edge : Connection
    {
        public int Id { get; internal set; }
        public int Player { get; set; }
        public float Current { get; private set; }
        public int Direction { get; internal set; }
        public Edge(Terminal a, Terminal b) : base(a, b) {}

        public virtual void SetCurrent(float current) => Current = current;
        public virtual void OnRemove() {}
    }

    [System.Serializable]
    public class Battery : Edge
    {
        public float Voltage { get; set; }
        public Battery(Terminal a, Terminal b) : base(a, b) {}
    }
    
    [System.Serializable]
    public class Resistor : Edge
    {
        public float Resistance { get; set; }
        // Id of series of resistors in the circuit
        internal int CombinedId { get; set; }
        // Combined resistance of a series of resistors
        internal float CombinedResistance { get; set; }
        public Resistor(Terminal a, Terminal b) : base(a, b) {}
    }
    
    [System.Serializable]
    public class Fuse : Resistor
    {
        public float MaxCurrent { get; set; }
        public Fuse(Terminal a, Terminal b) : base(a, b) {}
    }
}
