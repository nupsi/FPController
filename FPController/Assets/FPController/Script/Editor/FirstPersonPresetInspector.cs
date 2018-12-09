using UnityEditor;

namespace FPController.FPEditor
{
    /// <summary>
    /// Custom Inspector for First Person Preset.
    /// </summary>
    [CustomEditor(typeof(FirstPersonPreset))]
    public class FirstPersonPresetInspector : Editor
    {
        /// <summary>
        /// Current preset in inspected object.
        /// </summary>
        private FirstPersonPreset m_preset;

        private void OnEnable()
        {
            m_preset = Target;
        }

        public override void OnInspectorGUI()
        {
            FPEditorUtility.DrawPresetInspector(ref m_preset);
        }

        /// <summary>
        /// Editor target as first person preset.
        /// </summary>
        private FirstPersonPreset Target
        {
            get
            {
                return (FirstPersonPreset)target;
            }
        }
    }
}