using System.Collections.Generic;
using Game.Circuit;
using MessagePack;

namespace Game.Graphics
{
    [MessagePackObject]
    public class Terminal : ITerminal
    {
        [Key(0)] public int Id { get; set; }

        [IgnoreMember] public int Node { get; set; }
        [IgnoreMember] public List<IComponent> Components { get; set; }
    }

    [MessagePackObject]
    public class Battery : IVoltageSource
    {
        [IgnoreMember] public int VSourceId { get; set; }

        [Key(0)] public Terminal T1 { get; set; }
        [Key(1)] public Terminal T2 { get; set; }
        [Key(2)] public float Voltage { get; set; }

        public ITerminal GetT1() => T1;
        public ITerminal GetT2() => T2;
    }

    [MessagePackObject]
    public class Wire : IResistor
    {
        [Key(0)] public Terminal T1 { get; set; }
        [Key(1)] public Terminal T2 { get; set; }
        [Key(2)] public float Resistance { get; set; }

        public ITerminal GetT1() => T1;
        public ITerminal GetT2() => T2;
    }
    
    [MessagePackObject]
    public class Fuse : IResistor
    {
        [Key(0)] public Terminal T1 { get; set; }
        [Key(1)] public Terminal T2 { get; set; }
        [Key(2)] public float Resistance { get; set; }
        [Key(3)] public float MaxCurrent { get; set; }

        public ITerminal GetT1() => T1;
        public ITerminal GetT2() => T2;
    }

    public class Map
    {
    }
}
