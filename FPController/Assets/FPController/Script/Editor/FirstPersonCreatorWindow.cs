using Mouse;
using UnityEditor;
using UnityEngine;

namespace FPController.FPEditor
{
    /// <summary>
    /// Custom Editor Window for creating First Person Controller.
    /// </summary>
    public class FirstPersonCreatorWindow : EditorWindow
    {
        /// <summary>
        /// Current window instance.
        /// </summary>
        private static FirstPersonCreatorWindow m_window;

        /// <summary>
        /// Temporary preset to store properties during controller creation.
        /// </summary>
        private FirstPersonPreset m_preset;

        /// <summary>
        /// Scrollbar position.
        /// </summary>
        private Vector2 m_scroll;

        /// <summary>
        /// Is cursor controller requested.
        /// </summary>
        private bool m_addCursorController = false;

        [MenuItem("Extensions/First Person Controller")]
        private static void Open()
        {
            m_window = (FirstPersonCreatorWindow)GetWindow(typeof(FirstPersonCreatorWindow));
            m_window.titleContent = new GUIContent("FPController");
            m_window.Show();
        }

        private void OnGUI()
        {
            m_scroll = EditorGUILayout.BeginScrollView(m_scroll);
            GUILayout.Label("Create First Person Controller", EditorStyles.boldLabel);
            if(Preset != null)
            {
                FPEditorUtility.DrawPresetInspector(ref m_preset);
            }
            GUILayout.Space(10);
            m_addCursorController = EditorGUILayout.Toggle("Add Cursor Controller", m_addCursorController);
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Create First Person Controller"))
            {
                SavePrefab();
            }
        }

        /// <summary>
        /// Saves current controller configuration as prefabs.
        /// </summary>
        private void SavePrefab()
        {
            //Get save path from save file panel.
            var path = EditorUtility.SaveFilePanel("Save As prefab", Application.dataPath, Preset.Name, "prefab");
            if(path == string.Empty)
            {
                return;
            }
            else if(path.Contains(Application.dataPath))
            {
                //Convert path to local path.
                path = path.Replace(Application.dataPath, "Assets");

                //Create First Person Preset Scriptable object.
                AssetDatabase.CreateAsset(Preset, path.Replace(".prefab", " Preset.asset"));

                //Create game object for controller.
                var gameObject = new GameObject(Preset.Name);

                //Create camera for controller.
                var camera = CreateCamera(Preset.Name + " Camera");
                camera.transform.SetParent(gameObject.transform);

                //Add first person controller. This should also add rigidbody and capsule collider.
                var controller = gameObject.AddComponent<FirstPersonController>();
                controller.Settings = Preset;

                //Add cursor controller if it's checked.
                if(m_addCursorController)
                    gameObject.AddComponent<CursorController>();

                //Add input manager.
                gameObject.AddComponent<FirstPersonInputManager>();

                //Create Prefab from newly created player controller.
                PrefabUtility.CreatePrefab(path, gameObject);

                //Destroy temporary game object and scriptable object.
                DestroyImmediate(gameObject);
                m_preset = null;

                //Load Prefab from path.
                //This is done to keep the link between the prefab and scene.
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(path));
            }
            else
            {
                //Saving outside Application.dataPath is not allowed by PrefabUtility.CreatePrefab() because it uses 'local' path starting at Assets/.
                Debug.LogError("Saving outside Asset folder is not supported.\nTry saving inside folder: " + Application.dataPath);
            }
        }

        /// <summary>
        /// Creates GameObject with Camera attached.
        /// </summary>
        /// <param name="_name">Game objects name.</param>
        /// <returns>Game Object with camera components</returns>
        private GameObject CreateCamera(string _name)
        {
            var camera = new GameObject(_name);
            camera.AddComponent<Camera>();
            camera.AddComponent<FlareLayer>();
            camera.AddComponent<AudioListener>();
            return camera;
        }

        private FirstPersonPreset Preset
        {
            get
            {
                return m_preset ?? (m_preset = CreateInstance<FirstPersonPreset>());
            }
        }
    }
}