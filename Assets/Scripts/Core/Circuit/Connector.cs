using System.Collections.Generic;
using UnityEngine;

namespace Game.Circuit 
{
	public class Connector : MonoBehaviour
	{
		public float selectDistance = 5.0f;
		public Wire wirePrefab;
		public Terminal terminalPrefab;

		private List<Terminal> _terminals = new List<Terminal>();
		private Terminal _selected = null, _highlighted = null;

		private Camera _mainCamera = null;
		private Solver _solver = new Solver();

		private void Awake()
		{
			_mainCamera = Camera.main;

			// In a scene, all the terminals except the terminals created by wires, will already exist.
			// By finding and registering them in awake, saves the complexity of each terminal registering itself.		
			GameObject[] objs = GameObject.FindGameObjectsWithTag("Terminal"); // maybe use FindObjectsOfType instead ?
			foreach(var obj in objs) 
			{
				var terminal = obj.GetComponent<Terminal>();
				_terminals.Add(terminal);
				if(terminal.ground) _solver.grounds.Add(terminal);
			}
		}

		public void AddTerminal(Terminal terminal)
		{
			_terminals.Add(terminal);
		}

		// Linear. If needed can make this faster.
		private Terminal GetNearestTerminalToPoint(Vector3 point, Terminal exclude = null)
		{
			if(_terminals.Count == 0) return null;
			if(_terminals.Count == 1 && exclude != null) return null;

			Terminal res = _terminals[0];
			if(res == exclude)
			{
				res = _terminals[1];
			}
			foreach(Terminal term in _terminals)
			{
				if(term == exclude) continue;
				if(Vector3.Distance(term.transform.position, point) < Vector3.Distance(res.transform.position, point))
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
				var t = GetNearestTerminalToPoint(point);
				if(Vector3.Distance(point, t.transform.position) <= selectDistance)
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
					// if(!_selected.stayGround) wire.From.ground = false;

					if(_highlighted != null) 
					{
						wire.To = _highlighted;
					//	if(!_highlighted.stayGround) _highlighted.ground = false;
					}
					else
					{
						var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
						position.z = 0;
						Terminal terminal = GameObject.Instantiate(terminalPrefab);
						terminal.transform.position = position;
						terminal.ground = true;
						wire.To = terminal;
						AddTerminal(terminal);	
					}
					wire.Init();
					_solver.Solve();
				}
				_selected?.Highlight(false);
				_highlighted?.Highlight(false);
				_selected = _highlighted = null;
			}

			if(Input.GetMouseButton(0))
			{
				// highlight the nearest terminal
				var point = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
				point.z = 0;
				var t = GetNearestTerminalToPoint(point, _selected);
				
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
