using System.Collections.Generic;

namespace Game.Circuit
{
    public class Labeller
    {
        public static int LabelITerminals(IEnumerable<ITerminal> terminals, ITerminal ground)
        {
            foreach(ITerminal terminal in terminals) terminal.Node = -1;

            int nodes = 1;
            ground.Node = 0;

            var stack = new Stack<ITerminal>();
            stack.Push(ground);
            
            var visited = new HashSet<ITerminal>();
            while(stack.Count > 0)
            {
                ITerminal t = stack.Pop();
                foreach(IComponent comp in t.Components)
                {
                    ITerminal v = comp.GetT2();
                    if(t == comp.GetT2()) v = comp.GetT1();
                    if(visited.Contains(v)) continue;

                    v.Node = nodes++;
                    visited.Add(v);
                    stack.Push(v);
                }
            }
            return nodes;
        }

        public static int LabelVoltageSources(IEnumerable<IComponent> components)
        {
            int vSources = 0;
            foreach(IComponent comp in components)
            {
                if(comp is IVoltageSource source) source.VSourceId = vSources++;
            }
            return vSources;
        }
    }
}
