using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Circuit 
{
	public class Connector : MonoBehaviour
	{
		public float selectDistance = 5.0f;
		public Wire wirePrefab;
		public Terminal terminalPrefab;

		private List<Terminal> _terminals = new List<Terminal>();
		private List<IEdge> _defaultEdges = new List<IEdge>();
		private Terminal _selected = null, _highlighted = null;

		private Camera _mainCamera = null;

		// DOING Handle case of connecting to terminal of a Circuit component like Battery or Fuse.
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

					if(_selected.fakeGround) _selected.Unground();
					if(_highlighted != null)
					{
						wire.To = _highlighted;
						if(_highlighted.fakeGround) _highlighted.Unground();
					}
					else
					{
						var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
						position.z = 0;
						
						Terminal terminal = GameObject.Instantiate(terminalPrefab, position, terminalPrefab.transform.rotation);
						terminal.fakeGround = true;
					//	terminal.circuit = _selected.circuit; // Circuit already assigns this
						
						wire.To = terminal;
						AddTerminal(terminal);	
					}
					if(_highlighted.isComponentTerminal && wire.To.circuit == null)
					{
						wire.From.circuit.AddEdge(wire);
						wire.From.circuit.AddEdge(wire.To.Component);
					} else if(wire.To.circuit != wire.From.circuit)
					{
						// merge
					} else
					{
						wire.From.circuit.AddEdge(wire);
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
