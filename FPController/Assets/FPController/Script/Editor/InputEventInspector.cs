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
        private ReorderableList m_list;
        private List<ListItem> m_heights;

        private void OnEnable()
        {
            m_heights = new List<ListItem>();
            for(int i = 0; i < Target.EventCount; i++)
            {
                m_heights.Add(new ListItem());
            }

            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_events"), false, true, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight,
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
                },
                elementHeightCallback = (index) =>
                {
                    Repaint();
                    return m_heights[index].Expanded ? m_heights[index].Height : EditorGUIUtility.singleLineHeight;
                },
                onAddCallback = (list) =>
                {
                    var index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize++;
                    list.index = index;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("Name").stringValue = "Input Name";
                    element.FindPropertyRelative("KeyCode").enumValueIndex = 0;
                    element.FindPropertyRelative("CombinationKeyCode").enumValueIndex = 0;
                    element.FindPropertyRelative("KeyDownEvent").boolValue = false;
                    element.FindPropertyRelative("KeyEvent").boolValue = false;
                    element.FindPropertyRelative("KeyUpEvent").boolValue = false;
                    //TODO: Reset unity events, currently cloning events from previous event.
                    m_heights.Add(new ListItem());
                },
                onRemoveCallback = (list) =>
                {
                    m_heights.RemoveAt(list.index);
                    list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                }
            };
        }

        private void DrawListItemHeader(Rect _rect, SerializedProperty _element)
        {
            var rect = new Rect(_rect.x, _rect.y, _rect.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, _element.FindPropertyRelative("Name").stringValue);
            //var main = (KeyCode)_element.FindPropertyRelative("KeyCode").enumValueIndex;
            //var secondary = (KeyCode)_element.FindPropertyRelative("CombinationKeyCode").enumValueIndex;
            //var keyCodeText = string.Format("{0}{1}", secondary != KeyCode.None ? string.Format("{0} + ", secondary) : string.Empty, main);
            //rect.x += (_rect.width / 2);
            //EditorGUI.LabelField(rect, keyCodeText);
        }

        private void DrawListItemContent(Rect _rect, int _index, SerializedProperty _element)
        {
            if(_rect.width < 0)
                return;

            var lineHeight = EditorGUIUtility.singleLineHeight * 1.1f;
            var rect = new Rect(_rect.x, _rect.y + lineHeight, _rect.width, lineHeight);
            var singleLine = _rect.width < 600f;
            var height = lineHeight * 2;
            EditorGUIUtility.labelWidth = _rect.width / (singleLine ? 2 : 4);
            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("Name"), new GUIContent("Event Name"));
            var properties = new List<SerializedProperty[]>
            {
                new SerializedProperty[]
                {
                    _element.FindPropertyRelative("KeyCode"),
                    _element.FindPropertyRelative("CombinationKeyCode"),
                },
                new SerializedProperty[]
                {
                    _element.FindPropertyRelative("KeyDownEvent"),
                    _element.FindPropertyRelative("KeyEvent"),
                    _element.FindPropertyRelative("KeyUpEvent"),
                }
            };

            foreach(var line in properties)
            {
                if(!singleLine)
                {
                    rect.y += lineHeight;
                    height += lineHeight;
                }
                rect.x = _rect.x;
                rect.width = singleLine ? _rect.width : _rect.width / line.Length;
                foreach(var entry in line)
                {
                    if(singleLine)
                    {
                        rect.y += lineHeight;
                        height += lineHeight;
                    }
                    EditorGUI.PropertyField(rect, entry);
                    if(!singleLine)
                    {
                        rect.x += rect.width;
                    }
                }
            }

            rect.x = _rect.x;
            rect.width = _rect.width;
            rect.y += lineHeight;

            var events = new Dictionary<SerializedProperty, bool>
            {
                {
                    _element.FindPropertyRelative("GetKeyDown"),
                    _element.FindPropertyRelative("KeyDownEvent").boolValue
                },
                {
                    _element.FindPropertyRelative("GetKey"),
                    _element.FindPropertyRelative("KeyEvent").boolValue
                },
                {
                    _element.FindPropertyRelative("GetKeyUp"),
                    _element.FindPropertyRelative("KeyUpEvent").boolValue
                }
            };

            foreach(var entry in events)
            {
                if(entry.Value)
                {
                    EditorGUI.PropertyField(rect, entry.Key);
                    var addition = EditorGUI.GetPropertyHeight(entry.Key) + 15f;
                    height += addition;
                    rect.y += addition;
                }
            }

            m_heights[_index].Height = height;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_manager"));
            m_list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
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