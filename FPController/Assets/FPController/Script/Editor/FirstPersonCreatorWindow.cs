using Mouse;
using UnityEditor;
using UnityEngine;

namespace FPController
{
    public class FirstPersonCreatorWindow : EditorWindow
    {
        private static FirstPersonCreatorWindow m_window;
        private FirsPersonPreset m_preset;
        private Vector2 m_scroll;
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
            Preset.Name = EditorGUILayout.TextField("Name", Preset.Name);
            Preset.Tag = EditorGUILayout.TagField("Tag", Preset.Tag);
            GUILayout.Space(20);
            GUILayout.Label("Movement", EditorStyles.boldLabel);
            Preset.WalkSpeed = EditorGUILayout.Slider("Walk Speed", Preset.WalkSpeed, 0.2f, 10f);
            Preset.RunMultiplier = EditorGUILayout.Slider("Run Multiplier", Preset.RunMultiplier, 1f, 2.5f);
            Preset.CrouchMultiplier = EditorGUILayout.Slider("Crouch Multiplier", Preset.CrouchMultiplier, 0.1f, 1f);
            Preset.JumpForce = EditorGUILayout.FloatField("Jump Force", Preset.JumpForce);
            GUILayout.Space(20);
            GUILayout.Label("Measures", EditorStyles.boldLabel);
            Preset.Height = EditorGUILayout.Slider("Height", Preset.Height, 0.2f, 5f);
            Preset.Radius = EditorGUILayout.Slider("Radius", Preset.Radius, 0.1f, 2f);
            GUILayout.Space(20);
            GUILayout.Label("Other", EditorStyles.boldLabel);
            Preset.MaxSlopeAngle = EditorGUILayout.Slider("Max Slope Angle", Preset.MaxSlopeAngle, 0, 60f);
            EditorGUILayout.EndScrollView();
            GUILayout.Space(10);
            m_addCursorController = EditorGUILayout.Toggle("Add Cursor Controller", m_addCursorController);

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Create First Person Controller"))
            {
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

        private FirsPersonPreset Preset
        {
            get
            {
                return m_preset ?? (m_preset = CreateInstance<FirsPersonPreset>());
            }
        }
    }
}