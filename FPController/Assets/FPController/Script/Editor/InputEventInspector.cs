using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace FPController
{
    [CustomEditor(typeof(InputEvent))]
    public class InputEventInspector : Editor
    {
        private ReorderableList m_list;

        private void OnEnable()
        {
            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_events"), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Events");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.LabelField(
                        new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("Name").stringValue);
                    var keyCode = (KeyCode)element.FindPropertyRelative("KeyCode").enumValueIndex;
                    var combinationKey = (KeyCode)element.FindPropertyRelative("CombinationKeyCode").enumValueIndex;
                    var keyCodeText = string.Format("{0}{1}",
                        combinationKey != KeyCode.None ? string.Format("{0} + ", combinationKey) : string.Empty,
                        keyCode);
                    EditorGUI.LabelField(new Rect(rect.x + (rect.width / 2), rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), keyCodeText);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_manager"));
            m_list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}