using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FPController.FPEditor
{
    public class ListItem
    {
        public float Height;
        public bool Expanded;
        public ListItem() : this(EditorGUIUtility.singleLineHeight, false) { }
        public ListItem(float _height, bool _expanded)
        {
            Height = _height;
            Expanded = _expanded;
        }
    }

    [CustomEditor(typeof(InputEvent))]
    public class InputEventInspector : Editor
    {
        private static float ListLineHeight;
        private ReorderableList m_list;
        private List<ListItem> m_heights;

        private void OnEnable()
        {
            ListLineHeight = EditorGUIUtility.singleLineHeight * 1.1f;

            m_heights = new List<ListItem>();
            for(int i = 0; i < Target.EventCount; i++)
            {
                m_heights.Add(new ListItem());
            }

            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_events"), true, true, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 4,
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Events");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    DrawListItemHeader(rect, element);
                    m_heights[index].Expanded = isActive;
                    if(isActive)
                        DrawListItemContent(rect, index, element);
                    //EditorGUI.PropertyField(
                    //    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    //    element.FindPropertyRelative("KeyCode"), GUIContent.none);
                },
                elementHeightCallback = (index) =>
                {
                    Repaint();
                    return m_heights[index].Expanded ? m_heights[index].Height : ListLineHeight;
                }
            };
        }

        private void DrawListItemHeader(Rect _rect, SerializedProperty _element)
        {
            var rect = new Rect(_rect.x, _rect.y, _rect.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, _element.FindPropertyRelative("Name").stringValue);
            var main = (KeyCode)_element.FindPropertyRelative("KeyCode").enumValueIndex;
            var secondary = (KeyCode)_element.FindPropertyRelative("CombinationKeyCode").enumValueIndex;
            var keyCodeText = string.Format("{0}{1}", secondary != KeyCode.None ? string.Format("{0} + ", secondary) : string.Empty, main);
            rect.x += (_rect.width / 2);
            EditorGUI.LabelField(rect, keyCodeText);
        }

        private void DrawListItemContent(Rect _rect, int _index, SerializedProperty _element)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight * 1.1f;
            var rect = new Rect(_rect.x, _rect.y + ListLineHeight, _rect.width, lineHeight);
            var singleLine = _rect.width < 600f;
            var height = singleLine ? lineHeight * 3 : lineHeight * 2;
            EditorGUIUtility.labelWidth = _rect.width / (singleLine ? 2 : 4);

            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("Name"), new GUIContent("Event Name"));

            rect.y += lineHeight;
            rect.width = singleLine ? rect.width : rect.width / 2;
            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("KeyCode"));

            if(singleLine) rect.y += lineHeight;
            rect.x = singleLine ? rect.x : rect.x + rect.width;
            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("CombinationKeyCode"));

            rect.x = _rect.x;
            rect.width = _rect.width / 3;
            rect.y += lineHeight;

            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("KeyDownEvent"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("KeyEvent"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("KeyUpEvent"));


            rect.x = _rect.x;
            rect.y += lineHeight;
            rect.width = _rect.width;
            var eventHeight = 90;
            var heightAddition = 20;
            if(_element.FindPropertyRelative("KeyDownEvent").boolValue)
            {
                EditorGUI.PropertyField(rect, _element.FindPropertyRelative("GetKeyDown"));
                height += eventHeight + heightAddition;
                rect.y += eventHeight;
            }

            if(_element.FindPropertyRelative("KeyEvent").boolValue)
            {
                EditorGUI.PropertyField(rect, _element.FindPropertyRelative("GetKey"));
                height += eventHeight + heightAddition;
                rect.y += eventHeight;
            }

            if(_element.FindPropertyRelative("KeyUpEvent").boolValue)
            {
                EditorGUI.PropertyField(rect, _element.FindPropertyRelative("GetKeyUp"));
                height += eventHeight + heightAddition;
                rect.y += eventHeight;
            }

            m_heights[_index].Height = height;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //serializedObject.Update();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_manager"));
            //m_list.DoLayoutList();
            //serializedObject.ApplyModifiedProperties();
        }

        private InputEvent Target
        {
            get
            {
                return (InputEvent)target;
            }
        }
    }
}