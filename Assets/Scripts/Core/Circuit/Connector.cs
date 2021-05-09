using System.Collections.Generic;
using UnityEngine;

namespace Game.Circuit 
{
	// Handle ground terminals properly in Circuit.
	public class Connector : MonoBehaviour
	{
		public float selectDistance = 5.0f;
		public Wire wirePrefab;
		public Terminal terminalPrefab;

		private List<Terminal> _terminals = new List<Terminal>();
		private List<IEdge> _defaultEdges = new List<IEdge>();
		private Terminal _selected = null, _highlighted = null;

		private Camera _mainCamera = null;

		private void Awake()
		{
			_mainCamera = Camera.main;

			// In a scene, all the terminals except the terminals created by wires, will already exist.
			// By finding and registering them in awake, saves the complexity of each terminal registering itself.		
			GameObject[] objs = GameObject.FindGameObjectsWithTag("Terminal"); // maybe use FindObjectsOfType instead ?
			foreach(var obj in objs) 
			{
				Terminal terminal = obj.GetComponent<Terminal>();
				if(terminal.ground) terminal.circuit = new Circuit(terminal);
				AddTerminal(terminal);
			}


			// Setup circuit using preexisiting wires
			Wire[] wires = GameObject.FindObjectsOfType<Wire>();
			// do a bfs
			Queue<Terminal> q = new Queue<Terminal>();
			HashSet<Terminal> visited = new HashSet<Terminal>();
			foreach(Wire wire in wires) 
			{
				if(wire.To.ground) q.Enqueue(wire.To);
				if(wire.From.ground) q.Enqueue(wire.From);
			}
			while(q.Count > 0)
			{
				Terminal t = q.Dequeue();
				if(visited.Contains(t)) continue;

				if(t.isComponentTerminal && t.Component.Circuit == null)
				{
					t.circuit.AddEdge(t.Component);
					q.Enqueue(t.Component.To == t ? t.Component.From : t);
				}
				visited.Add(t);
				foreach(Wire wire in wires)
				{
					if(wire.To == t || wire.From == t)
					{
						Terminal to = wire.To == t ? wire.From : wire.To;
						if(to.circuit == null)
						{
							t.circuit.AddEdge(wire);
							q.Enqueue(to);
						}
					}
				}
			}
			foreach(Terminal term in visited) term.circuit.Solve();
		}

		public void AddTerminal(Terminal terminal)
		{
			_terminals.Add(terminal);
		}

		/// <param name="Point">World space vector3. But works best if point.z = 0.</param>
		/// <param name="allowOrphan">Whether to allow returning terminal which is not part of a circuit.</param>
		/// <param name="exclude">Terminal to exclude, it also excludes any terminal which is part of same circuit and is of same node.</param>
		private Terminal GetNearestTerminalToPoint(Vector3 point, bool allowOrphan, Terminal exclude = null)
		{
			if(_terminals.Count == 0) return null;
			if(_terminals.Count == 1) return null;
			Terminal res = null;

			foreach(Terminal term in _terminals)
			{
				// cannot select terminal not part of a circuit
				if(!allowOrphan && term.circuit == null) continue;
				if(exclude != null && (term.circuit == exclude.circuit && term.Node == exclude.Node)) continue;
				if(res == null || Vector3.Distance(res.transform.position, point) > Vector3.Distance(term.transform.position, point))
					res = term;
			}
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
				point.z = 0;
				var t = GetNearestTerminalToPoint(point, false);
				if(t != null && Vector3.Distance(point, t.transform.position) <= selectDistance)
				{
					_selected = t;
					t.Highlight(true);
				}
			}

			if(Input.GetMouseButtonUp(0))
			{
				// join selected to highlighted
				if(_selected != null)
				{
					Wire wire = GameObject.Instantiate(wirePrefab);
					wire.From = _selected;

					Circuit circuit = _selected.circuit;
					if(_highlighted != null)
					{
						wire.To = _highlighted;
					}
					else
					{
						var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
						position.z = 0;
						
						Terminal terminal = GameObject.Instantiate(terminalPrefab, position, terminalPrefab.transform.rotation);
						
						wire.To = terminal;
						AddTerminal(terminal);	
					}
					if(wire.To.circuit != null && wire.To.circuit != wire.From.circuit)
					{
						// merge
					} else 
					{
						// only add component edge if the terminals are not already part of the same circuit.
						if(wire.To.circuit == null && wire.To.isComponentTerminal) circuit.AddEdge(wire.To.Component);
						
						// add component edge before this
						circuit.AddEdge(wire);
						circuit.Solve();
					}
					wire.Init();
				}
				_selected?.Highlight(false);
				_highlighted?.Highlight(false);
				_selected = _highlighted = null;
			}

			if(Input.GetMouseButton(0) && _selected != null)
			{
				// highlight the nearest terminal
				var point = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
				point.z = 0;
				var t = GetNearestTerminalToPoint(point, true, _selected);
				
				_highlighted?.Highlight(false);
				_highlighted = null;
				if(Vector3.Distance(point, t.transform.position) <= selectDistance)
				{
					t.Highlight(true);
					_highlighted = t;
				}
			}
		}
	}
}
