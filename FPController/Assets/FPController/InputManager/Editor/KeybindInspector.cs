using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FPController.FPEditor
{
    [CustomEditor(typeof(ScriptableKeybind))]
    public class KeybindInspector : Editor
    {
        /// <summary>
        /// Reorderable list for scriptable keybind key codes.
        /// </summary>
        private ReorderableList m_keyCodes;

        /// <summary>
        /// Reorderable list for scriptable keybind combination key codes.
        /// </summary>
        private ReorderableList m_combinationKeyCodes;

        protected void OnEnable()
        {
            m_keyCodes = new ReorderableList(serializedObject, serializedObject.FindProperty("KeyCodes"), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => DrawHeaderCallback(rect, "Key Codes"),
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)
                    => DrawElementCallback(rect, index, m_keyCodes)
            };

            m_combinationKeyCodes = new ReorderableList(serializedObject, serializedObject.FindProperty("CombinationKeyCodes"), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => DrawHeaderCallback(rect, "Combination Key Codes"),
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)
                    => DrawElementCallback(rect, index, m_combinationKeyCodes)
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Target.Name = EditorGUILayout.TextField("Keybind Name", Target.Name);
            m_keyCodes.DoLayoutList();
            m_combinationKeyCodes.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw header for reorderable list at given position with the given text.
        /// </summary>
        /// <param name="rect">Header position.</param>
        /// <param name="text">Header text.</param>
        private void DrawHeaderCallback(Rect rect, string text)
        {
            EditorGUI.LabelField(rect, text);
        }

        /// <summary>
        /// Draw reorderable list element at given position for a given list.
        /// </summary>
        /// <param name="rect">Current element position.</param>
        /// <param name="index">Current element index in the list.</param>
        /// <param name="list">Current list.</param>
        private void DrawElementCallback(Rect rect, int index, ReorderableList list)
        {
            rect.y += 2;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, new GUIContent("Key Code"));
        }

        /// <summary>
        /// Current target element being inspected.
        /// </summary>
        private ScriptableKeybind Target
        {
            get
            {
                return target as ScriptableKeybind;
            }
        }
    }
}