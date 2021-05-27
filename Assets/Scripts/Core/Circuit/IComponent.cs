using System.Collections.Generic;

namespace Game.Circuit
{
    public interface ITerminal
    {
        public int Node { get; set; }
        public List<IComponent> Components { get; }
    }

    public interface IComponent
    {
        ITerminal GetT1();
        ITerminal GetT2();
    }

    public static class IComponentExtensions
    {
        public static int GetNode(this IComponent component, int node)
        {
            if(node < 0) return component.GetT2().Node;
            return component.GetT1().Node;
        }
    }

    public interface IVoltageSource : IComponent
    {
        int VSourceId { get; set; }
        float Voltage { get; }
    }

    public interface IResistor : IComponent
    {
        float Resistance { get; }
    }
}
