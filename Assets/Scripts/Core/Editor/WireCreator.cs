using UnityEngine;
using UnityEditor;
using System.Linq;
using Game.Circuit;

namespace Game.Editor
{
	public class WireCreator : EditorWindow
	{
		private static Wire _wirePrefab;
		[MenuItem("Game/Wire Creator")]
		private static void Init()
		{
			WireCreator window = EditorWindow.GetWindow<WireCreator>();
			window.Show();
		}
		
		private void OnGUI()
		{
			_wirePrefab = (Wire)EditorGUILayout.ObjectField("Wire Prefab", _wirePrefab, typeof(Wire), false);
			if(_wirePrefab == null) return;

			if(GUILayout.Button("Create"))
			{
				var selection = Selection.GetTransforms(SelectionMode.ExcludePrefab)
								.Where(t => t.GetComponent<Terminal>() != null)
								.Select(t => t.GetComponent<Terminal>())
								.ToList();
				if(selection.Count == 2)
				{
					Wire wire = (Wire)PrefabUtility.InstantiatePrefab(_wirePrefab);
					wire.From = selection[0];
					wire.To = selection[1];
					wire.Init();

					Undo.RegisterCreatedObjectUndo(wire.gameObject, "Connect terminals.");
				}
			}
		}
	}
}
