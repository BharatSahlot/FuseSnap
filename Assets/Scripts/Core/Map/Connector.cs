using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Circuit;

namespace Game.Map
{
    // Handle ground terminals properly in Circuit.
    public class Connector : MonoBehaviour
	{
		public float selectDistance = 5.0f;
		public Wire wirePrefab;
		public MonoTerminal terminalPrefab;
        [SerializeField] private Color[] _colors;

		private List<MonoTerminal> _terminals = new List<MonoTerminal>();
		private MonoTerminal _selected = null, _highlighted = null;

		private Camera _mainCamera = null;
		private Circuit.Circuit _circuit;
        private int _currentPlayer = 0;


		private void Awake()
		{
			_mainCamera = Camera.main;

			// In a scene, all the terminals except the terminals created by wires, will already exist.
			// By finding and registering them in awake, saves the complexity of each terminal registering itself.		
            GameObject.FindGameObjectsWithTag("Terminal").ToList().ForEach(t => AddTerminal(t.GetComponent<MonoTerminal>()));
		}

		public void AddTerminal(MonoTerminal terminal) 
        {
            _terminals.Add(terminal);
            terminal.onDestroyed += () => RemoveTerminal(terminal);
        }
        public void RemoveTerminal(MonoTerminal terminal) => _terminals.Remove(terminal);

		/// <param name="Point">World space vector3. But works best if point.z = 0.</param>
		/// <param name="allowOrphan">Whether to allow returning terminal which is not part of a circuit.</param>
		private MonoTerminal GetNearestTerminalToPoint(Vector3 point, bool allowOrphan, int excludeNode = -1)
		{
			MonoTerminal res = null;
			foreach(MonoTerminal term in _terminals)
			{
				if(_circuit.Ground == term.Terminal) continue; // players cant connect to grounds
				// dont select terminal not part of a circuit
				if(!allowOrphan && !_circuit.HasTerminal(term.Terminal)) continue;
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
			foreach(var t in _terminals)
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

        private void CreateWire(MonoTerminal A, MonoTerminal B)
        {
            Wire wire = GameObject.Instantiate(wirePrefab);
            Edge edge = new Edge(A.Terminal, B.Terminal);
            wire.A = A;
            wire.B = B;
            // wire.GetComponent<LineRenderer>().startColor = _colors[B.Terminal.Player];
            // wire.GetComponent<LineRenderer>().endColor = _colors[A.Terminal.Player];
            _circuit.AddEdge(edge);
        }

        private void EndSelection()
        {
            // join selected to highlighted
            if (_selected != null)
            {
                if(_highlighted != null)
                {
                    if(_circuit.CanAddEdge(_selected.Terminal, _highlighted.Terminal))
                    {
                        // if(_highlighted.Terminal.Player == -1) _highlighted.Terminal.Player = _currentPlayer;
                        // if(_highlighted.Terminal.Component != null)
                        // {
                        //     if(_highlighted.Terminal.Component[0].Player == -1) _highlighted.Terminal.Component[0].Player = _currentPlayer;
                        //     if(_highlighted.Terminal.Component[1].Player == -1) _highlighted.Terminal.Component[1].Player = _currentPlayer;
                        // }
                        CreateWire(_selected, _highlighted);
                    }
                } else 
                {
                    var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    MonoTerminal terminal = GameObject.Instantiate(terminalPrefab, position, terminalPrefab.transform.rotation);
                    terminal.Terminal = new Terminal();
                    // terminal.Terminal.Player = _currentPlayer;
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
