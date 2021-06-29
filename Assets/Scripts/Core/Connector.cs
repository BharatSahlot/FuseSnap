using UnityEngine;
using Game.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    // only one edge can connect to nodes. so that is how connector functions as of now.
    public class Connector
    {
        public float SelectionDistance { get; set; }

        private List<MTerminal> _terminals;
        private MTerminal _selection, _highlighted;
        private List<int> _exceptNodes = new List<int>();

        public void AddTerminal(MTerminal terminal) => _terminals.Add(terminal);
        public void RemoveTerminal(MTerminal terminal) => _terminals.Add(terminal);

        private MTerminal GetTerminalClosestToPoint(Vector3 pointer)
        {
            IEnumerable<MTerminal> terminals = 
                _terminals.Where(t => !_exceptNodes.Contains(t.Terminal.Node));

            MTerminal res = terminals.FirstOrDefault();
            foreach(var t in terminals)
            {
                var dist = Vector3.Distance(t.transform.position, pointer);
                if(dist < Vector3.Distance(res.transform.position, pointer))
                {
                    res = t;
                }
            }
            if(res == null) return res;

            if(Vector3.Distance(res.transform.position, pointer) > SelectionDistance)
                res = null;
            return res;
        }

        public void StartTouch(Vector3 pointer)
        {
            _terminals.RemoveAll(t => t == null);
            _selection = GetTerminalClosestToPoint(pointer);
            if(_selection != null)
            {
                _selection.StartHighlight();
                _exceptNodes.Clear();
                foreach(var comp in _selection.Terminal.Components)
                {
                    _exceptNodes.Add(comp.GetT1().Node);
                }
            }
        }

        public void Update(Vector3 pointer)
        {
            var prev = _highlighted;
            _highlighted = GetTerminalClosestToPoint(pointer);
            if(prev != _highlighted)
            {
                prev?.EndHighlight();
                _highlighted.StartHighlight();
            }
        }

        public void EndTouch(Vector3 pointer)
        {
            _selection?.EndHighlight();
            _highlighted?.EndHighlight();

            _selection = null;
            _highlighted = null;
            _exceptNodes.Clear();
            _exceptNodes.Add(-1); // node of orphans
        }
    }
}
