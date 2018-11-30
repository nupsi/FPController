using UnityEditor;

namespace FPController
{
    [CustomEditor(typeof(FirsPersonPreset))]
    public class FirstPersonPresetInspector : Editor
    {
        private FirsPersonPreset m_preset;

        private void OnEnable()
        {
            m_preset = Target;
        }

        public override void OnInspectorGUI()
        {
            FPEditorUtility.DrawPresetInspector(ref m_preset);
        }

        private FirsPersonPreset Target
        {
            get
            {
                return (FirsPersonPreset)target; 
            }
        }
    }
}