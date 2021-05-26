using UnityEditor;
using UnityEngine;

namespace Game.Map.Editor
{
    public class Designer : EditorWindow
    {
        // c# gib enum inheritance
        private enum DrawableTypes // : ComponentTypes
        {
            none, 
            Battery,
            Fuse,
            Wire
        }
        
        private SerializedObject _obj;
        [SerializeField] private Map _map;
        [SerializeField] private float _squareSize = 5.0f;
        [SerializeField] private GameObject _batteryPrefab;
        [SerializeField] private GameObject _fusePrefab;
        [SerializeField] private GameObject _wirePrefab;
        [SerializeField] private GameObject _terminalPrefab;
        [SerializeField ]private DrawableTypes _type = DrawableTypes.none;

        private bool _show;

        [MenuItem("Game/Designer")]
        public static void Init()
        {
            Designer window = EditorWindow.GetWindow<Designer>();
            window.Show();
            window._obj = new SerializedObject(window);
        }

        private void OnGUI()
        {
            if(_obj == null) _obj = new SerializedObject(this);

            _obj.Update();
            
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(_obj.FindProperty("_type"), new GUIContent("Element To Draw: "), true);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(_obj.FindProperty("_squareSize"), new GUIContent("Square Size"), true);
            EditorGUILayout.PropertyField(_obj.FindProperty("_map"), new GUIContent("Map"), true);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            if(_show = EditorGUILayout.Foldout(_show, "Prefabs")) 
            {
                EditorGUILayout.PropertyField(_obj.FindProperty("_batteryPrefab"), new GUIContent("Battery"), true);
                EditorGUILayout.PropertyField(_obj.FindProperty("_fusePrefab"), new GUIContent("Fuse"), true);
                EditorGUILayout.PropertyField(_obj.FindProperty("_wirePrefab"), new GUIContent("Wire"), true);
                EditorGUILayout.PropertyField(_obj.FindProperty("_terminalPrefab"), new GUIContent("Terminal"), true);
            }
            EditorGUILayout.EndVertical();
            _obj.ApplyModifiedProperties();
        }

        private void CreateComponent(DrawableTypes type, int r, int c, Vector3 position)
        {
            if(type == DrawableTypes.Fuse) 
            {
                var obj = GameObject.Instantiate(_fusePrefab);
                obj.transform.position = position;

                // _map.Components.Add(new CircuitComponent() 
                //         {
                //             R = r,
                //             C = c,
                //             Rotation = 0,
                //             Resistance = 1,
                //             MaxCurrent = 10
                //         });
            } else if(type == DrawableTypes.Battery)
            {
                var obj = GameObject.Instantiate(_batteryPrefab); 
                obj.transform.position = position;

                // _map.Components.Add(new CircuitComponent() 
                //         {
                //             R = r,
                //             C = c,
                //             Rotation = 0,
                //             Voltage = 10
                //         });
            }
        }

        private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
        private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;
        private void OnSceneGUI(SceneView sceneView)
        {
            if(SceneView.lastActiveSceneView == null) return;

            Plane plane = new Plane(Vector3.up, 0);
            Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

            int r = (int)(mousePosition.x / _squareSize);
            int c = (int)(mousePosition.y / _squareSize);
            for(int i = 0; i < _map.R; ++i)
            {
                for(int j = 0; j < _map.C; ++j)
                {
                    Rect rect = new Rect(i * _squareSize, j * _squareSize, _squareSize, _squareSize);
                    if(i == r && j == c) 
                    {
                        if(Event.current.type == EventType.MouseDown) 
                        {
                            switch(_type) 
                            {
                                case DrawableTypes.none: break;
                                case DrawableTypes.Wire: break;
                                default:
                                {
                                    CreateComponent(_type, i, j, new Vector3(i * _squareSize + _squareSize / 2.0f, j * _squareSize + _squareSize / 2.0f, 0));
                                    break;
                                }
                            }
                        }
                        Handles.DrawSolidRectangleWithOutline(rect, "#9FCC59".ToColor(), "#A7D65D".ToColor());
                    } else 
                    {
                        Handles.DrawSolidRectangleWithOutline(rect, "#6A883B".ToColor(), "#A7D65D".ToColor());
                    }
                }
            }

            Handles.BeginGUI();
            GUILayout.Label("Hello");
            Handles.EndGUI();
            SceneView.lastActiveSceneView.Repaint();
        }
    }
}
