using UnityEditor;
using UnityEngine;

namespace FPController.FPEditor
{
    /// <summary>
    /// Custom inspector for First Person Controller.
    /// Provide custom inspector for preset.
    /// </summary>
    [CustomEditor(typeof(FirstPersonController))]
    public class FirstPersonControllerInspector : Editor
    {
        /// <summary>
        /// Local copy of controller preset.
        /// </summary>
        private FirstPersonPreset m_preset;

        private void OnEnable()
        {
            m_preset = Preset;
        }

        public override void OnInspectorGUI()
        {
            //Draw default inspector to display field for preset.
            DrawDefaultInspector();

            //Draw inspector for current preset, if current target has one.
            if(Target.Settings != null)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                FPEditorUtility.DrawPresetInspector(ref m_preset);
                if(GUI.changed)
                {
                    Preset = m_preset;
                }
            }
        }

        /// <summary>
        /// Current target controller.
        /// </summary>
        private FirstPersonController Target
        {
            get
            {
                return target as FirstPersonController;
            }
        }

        /// <summary>
        /// Current selection preset.
        /// </summary>
        private FirstPersonPreset Preset
        {
            get
            {
                return Target.Settings;
            }

            set
            {
                Target.Settings = value;
            }
        }
    }
}