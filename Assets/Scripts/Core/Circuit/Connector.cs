using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Circuit
{
    // Handle ground terminals properly in Circuit.
    public class Connector : MonoBehaviour
	{
		public float selectDistance = 5.0f;
		public Wire wirePrefab;
		public Terminal terminalPrefab;
        [SerializeField] private Color[] _colors;

		private List<Terminal> _terminals = new List<Terminal>();
		private Terminal _selected = null, _highlighted = null;

		private Camera _mainCamera = null;
		private Circuit _circuit = new Circuit();
        private int _currentPlayer = 0;


		private void Awake()
		{
			_mainCamera = Camera.main;

			// In a scene, all the terminals except the terminals created by wires, will already exist.
			// By finding and registering them in awake, saves the complexity of each terminal registering itself.		
            GameObject.FindGameObjectsWithTag("Terminal").ToList().ForEach(t => AddTerminal(t.GetComponent<Terminal>()));

			// Setup circuit using preexisiting wires
			Wire[] wires = GameObject.FindObjectsOfType<Wire>();
			// do a bfs
			Queue<Terminal> q = new Queue<Terminal>();
			HashSet<Terminal> visited = new HashSet<Terminal>();
			foreach(Wire wire in wires) 
			{
				if(wire.To.ground) q.Enqueue(wire.To);
				if(wire.From.ground) q.Enqueue(wire.From);

                wire.GetComponent<LineRenderer>().startColor = _colors[wire.From.player];
                wire.GetComponent<LineRenderer>().endColor = _colors[wire.To.player];
			}
			while(q.Count > 0)
			{
				Terminal t = q.Dequeue();
				if(visited.Contains(t)) continue;

				if(t.Component != null && _circuit.AddEdge(t.Component))
				{
                    q.Enqueue(t.Component.To == t ? t.Component.From : t.Component.To);
				}
				visited.Add(t);
				foreach(Wire wire in wires)
				{
					if(wire.To == t || wire.From == t)
					{
						Terminal to = wire.To == t ? wire.From : wire.To;
						if(!visited.Contains(to))
						{
							_circuit.AddEdge(wire);
							q.Enqueue(to);
						}
					}
				}
			}
			_circuit.Update();
		}

		public void AddTerminal(Terminal terminal) 
        {
            _terminals.Add(terminal);
            terminal.onDestroyed += () => RemoveTerminal(terminal);
        }
        public void RemoveTerminal(Terminal terminal) => _terminals.Remove(terminal);

		/// <param name="Point">World space vector3. But works best if point.z = 0.</param>
		/// <param name="allowOrphan">Whether to allow returning terminal which is not part of a circuit.</param>
		private Terminal GetNearestTerminalToPoint(Vector3 point, bool allowOrphan, int excludeNode = -1)
		{
			Terminal res = null;
			foreach(Terminal term in _terminals)
			{
				if(term.ground) continue; // players cant connect to grounds
				// dont select terminal not part of a circuit
				if(!allowOrphan && !_circuit.HasTerminal(term)) continue;
				if(term == _selected) continue;
				if(res == null || Vector3.Distance(res.transform.position, point) > Vector3.Distance(term.transform.position, point))
					res = term;
			}
            if(res == null || Vector3.Distance(res.transform.position, point) > selectDistance) return null;
			return res;
		}

		private void Update()
		{
#if UNITY_EDITOR
			foreach(Terminal t in _terminals)
			{
				var point = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
				point.z = 0;
				if(Vector3.Distance(t.transform.position, point) <= selectDistance)
				{
					Debug.DrawLine(t.transform.position, point, Color.green);
				} else
				{
					Debug.DrawLine(t.transform.position, point, Color.red);
				}
			}
#endif

			if(Input.GetMouseButtonDown(0))
            {
                var point = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                BeginSelection(point);
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndSelection();
            }

            if (Input.GetMouseButton(0) && _selected != null)
            {
                // highlight the nearest terminal
                var point = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                UpdateSelection(point);
            }
        }

        private void CreateWire(Terminal from, Terminal to)
        {
            Wire wire = GameObject.Instantiate(wirePrefab);
            wire.From = from;
            wire.To = to;
            wire.GetComponent<LineRenderer>().startColor = _colors[wire.To.player];
            wire.GetComponent<LineRenderer>().endColor = _colors[wire.From.player];
            if (_circuit.AddEdge(wire))
            {
                if (wire.To.Component != null) _circuit.AddEdge(wire.To.Component);
                if (wire.From.Component != null) _circuit.AddEdge(wire.From.Component);
                _circuit.Update();
            }
        }

        private void EndSelection()
        {
            // join selected to highlighted
            if (_selected != null)
            {
                if(_highlighted != null)
                {
                    if(_circuit.CanAddEdge(_selected, _highlighted))
                    {
                        if(_highlighted.player == -1) _highlighted.player = _currentPlayer;
                        if(_highlighted.Component != null)
                        {
                            if(_highlighted.Component.To.player == -1) _highlighted.Component.To.player = _currentPlayer;
                            if(_highlighted.Component.From.player == -1) _highlighted.Component.From.player = _currentPlayer;
                        }
                        CreateWire(_selected, _highlighted);
                    }
                } else 
                {
                    var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Terminal terminal = GameObject.Instantiate(terminalPrefab, position, terminalPrefab.transform.rotation);
                    terminal.player = _currentPlayer;
                    AddTerminal(terminal);
                    CreateWire(_selected, terminal);
                }
                _currentPlayer ^= 1;
            }
            _selected?.Highlight(false);
            _highlighted?.Highlight(false);
            _selected = _highlighted = null;
        }

        private void UpdateSelection(Vector3 point)
        {
            point.z = 0;

            var t = GetNearestTerminalToPoint(point, true);

            _highlighted?.Highlight(false);
            _highlighted = null;
            if(t != null)
            {
                t.Highlight(true);
                _highlighted = t;
            }
        }

        private void BeginSelection(Vector3 point)
        {
            point.z = 0;
            var t = GetNearestTerminalToPoint(point, false);
            if (t != null)
            {
                _selected = t;
                t.Highlight(true);
            }
        }
    }
}
