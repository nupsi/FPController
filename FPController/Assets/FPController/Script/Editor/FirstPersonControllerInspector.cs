using UnityEditor;
using UnityEngine;

namespace FPController
{
    [CustomEditor(typeof(FirstPersonController))]
    public class FirstPersonControllerInspector : Editor
    {
        private FirsPersonPreset m_preset;

        private void OnEnable()
        {
            m_preset = Preset;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

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

        private FirstPersonController Target
        {
            get
            {
                return target as FirstPersonController;
            }
        }

        private FirsPersonPreset Preset
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