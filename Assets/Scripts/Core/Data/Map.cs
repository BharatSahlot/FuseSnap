using System.Collections.Generic;
using Game.Circuit;
using MessagePack;

namespace Game.Data
{
    public class Terminal : ITerminal
    {
        public int Id { get; set; }

        [IgnoreMember] public int Node { get; set; }
        [IgnoreMember] public List<IComponent> Components { get; set; }
    }

    [MessagePackObject]
    public class Battery : IVoltageSource, IMessagePackSerializationCallbackReceiver
    {
        [IgnoreMember] public int VSourceId { get; set; }

        [IgnoreMember] public Terminal T1 { get; set; }
        [IgnoreMember] public Terminal T2 { get; set; }

        [Key(0)] public int T1Id { get; private set; }
        [Key(1)] public int T2Id { get; private set; }
        [Key(2)] public float Voltage { get; set; }
        [Key(3)] public float Rotation { get; set; }

        public ITerminal GetT1() => T1;
        public ITerminal GetT2() => T2;

        public void OnAfterDeserialize() { }
        public void OnBeforeSerialize()
        {
            T1Id = T1.Id;
            T2Id = T2.Id;
        }
    }

    [MessagePackObject]
    public class Wire : IResistor, IMessagePackSerializationCallbackReceiver
    {
        [IgnoreMember] public Terminal T1 { get; set; }
        [IgnoreMember] public Terminal T2 { get; set; }

        [Key(0)] public int T1Id { get; private set; }
        [Key(1)] public int T2Id { get; private set; }
        [Key(2)] public float Resistance { get; set; }

        public ITerminal GetT1() => T1;
        public ITerminal GetT2() => T2;

        public void OnAfterDeserialize() { }
        public void OnBeforeSerialize()
        {
            T1Id = T1.Id;
            T2Id = T2.Id;
        }
    }
    
    [MessagePackObject]
    public class Fuse : IResistor, IMessagePackSerializationCallbackReceiver
    {
        [IgnoreMember] public Terminal T1 { get; set; }
        [IgnoreMember] public Terminal T2 { get; set; }

        [Key(0)] public int T1Id { get; private set; }
        [Key(1)] public int T2Id { get; private set; }
        [Key(2)] public float Resistance { get; set; }
        [Key(3)] public float MaxCurrent { get; set; }
        [Key(4)] public float Rotation { get; set; }

        public ITerminal GetT1() => T1;
        public ITerminal GetT2() => T2;

        public void OnAfterDeserialize() { }
        public void OnBeforeSerialize()
        {
            T1Id = T1.Id;
            T2Id = T2.Id;
        }
    }

    [MessagePackObject]
    public class Map : IMessagePackSerializationCallbackReceiver
    {
        [Key(0)] public int R { get; set; }
        [Key(1)] public int C { get; set; }

        [Key(2)] public List<Battery> Batteries { get; set; }
        [Key(3)] public List<Fuse> Fuses { get; set; }
        [Key(4)] public List<Wire> Wires { get; set; }

        public void OnAfterDeserialize()
        {
            var terminals = new List<Terminal>();
            Terminal GetTerminal(int id) 
            {
                Terminal res = terminals.Find(t => t.Id == id);
                if(res == null)
                {
                    res = new Terminal();
                    res.Id = id;
                }
                return res;
            }

            foreach(Battery battery in Batteries)
            {
                battery.T1 = GetTerminal(battery.T1Id);
                battery.T2 = GetTerminal(battery.T2Id);
            }
            foreach(Fuse fuse in Fuses)
            {
                fuse.T1 = GetTerminal(fuse.T1Id);
                fuse.T2 = GetTerminal(fuse.T2Id);
            }
            foreach(Wire wire in Wires)
            {
                wire.T1 = GetTerminal(wire.T1Id);
                wire.T2 = GetTerminal(wire.T2Id);
            }
        }

        public void OnBeforeSerialize() { }
    }
}
