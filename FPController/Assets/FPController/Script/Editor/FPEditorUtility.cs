using UnityEditor;

namespace FPController
{
    public static class FPEditorUtility
    {
        public static void DrawPresetInspector(ref FirsPersonPreset _preset)
        {
            EditorGUILayout.LabelField("Controller Settings", EditorStyles.boldLabel);
            _preset.Name = EditorGUILayout.TextField("Name", _preset.Name);
            _preset.Tag = EditorGUILayout.TagField("Tag", _preset.Tag);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            _preset.WalkSpeed = EditorGUILayout.Slider("Walk Speed", _preset.WalkSpeed, 0.2f, 10f);
            _preset.RunMultiplier = EditorGUILayout.Slider("Run Multiplier", _preset.RunMultiplier, 1f, 2.5f);
            _preset.CrouchMultiplier = EditorGUILayout.Slider("Crouch Multiplier", _preset.CrouchMultiplier, 0.1f, 1f);
            _preset.JumpForce = EditorGUILayout.FloatField("Jump Force", _preset.JumpForce);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
            _preset.Height = EditorGUILayout.Slider("Height", _preset.Height, 0.2f, 5f);
            _preset.Radius = EditorGUILayout.Slider("Radius", _preset.Radius, 0.1f, 2f);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other", EditorStyles.boldLabel);
            _preset.MaxSlopeAngle = EditorGUILayout.Slider("Max Slope Angle", _preset.MaxSlopeAngle, 0, 60f);
        }
    }
}