using UnityEngine;
using Game.Graphics;
using System.Collections.Generic;

namespace Game
{
    // THINK about disconnected terminals and stuff
    public class Connector
    {
        public float SelectionDistance { get; set; }

        private List<MTerminal> _terminals;
        private MTerminal _selection, _highlighted;

        private MTerminal GetTerminalClosestToPoint(Vector3 pointer)
        {
            MTerminal res = _terminals[0];
            foreach(var t in _terminals)
            {
                var dist = Vector3.Distance(t.transform.position, pointer);
                if(dist < Vector3.Distance(res.transform.position, pointer))
                {
                    res = t;
                }
            }
            if(Vector3.Distance(res.transform.position, pointer) > SelectionDistance)
                res = null;
            return res;
        }

        public void StartTouch(Vector3 pointer)
        {
            _selection = GetTerminalClosestToPoint(pointer);
            if(_selection != null)
            {
                _selection.StartHighlight();
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
        }
    }
}