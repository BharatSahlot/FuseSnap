using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace Game.Circuit
{
    public enum EdgeTypes
    {
        Wire, Battery, Fuse
    }

    public class Terminal
    {
        public int Id { get; internal set; }
        public int Node { get; internal set; }
        public float Voltage { get; internal set; }
        public Edge Component { get; internal set; }
        public List<Edge> Edges { get; internal set; }
        
        public virtual void OnRemove() {}
    }

    public class Edge : Connection
    {
        public int Id { get; internal set; }
        public float Current { get; private set; }
        public int Direction { get; internal set; }
        public Edge(Terminal a, Terminal b) : base(a, b) {}

        public virtual void SetCurrent(float current) => Current = current;
        public virtual void OnRemove() {}
    }

    [System.Serializable, MessagePackObject]
    public class Battery : Edge
    {
        [Key(0), SerializeField]
        public float Voltage { get; set; }
        public Battery(Terminal a, Terminal b) : base(a, b) {}
    }

    [MessagePack.Union(0, typeof(Wire))]
    [MessagePack.Union(1, typeof(Fuse))]
    public interface IResistor 
    {
        [Key(0), SerializeField]
        float Resistance { get; set; }
        
        int CombinedId { get; set; }
        float CombinedResistance { get; set; }
    }
    
    [System.Serializable, MessagePackObject]
    public class Wire : Edge, IResistor
    {
        public float Resistance { get; set; }
        // Id of series of resistors in the circuit
        public int CombinedId { get; set; }
        // Combined resistance of a series of resistors
        public float CombinedResistance { get; set; }
        public Wire(Terminal a, Terminal b) : base(a, b) {}
    }
    
    [System.Serializable, MessagePackObject]
    public class Fuse : Edge, IResistor
    {
        public float Resistance { get; set; }
        
        [Key(1), SerializeField]
        public float MaxCurrent { get; set; }

        public int CombinedId { get; set; }
        public float CombinedResistance { get; set; }

        public Fuse(Terminal a, Terminal b) : base(a, b) {}
    }
}
