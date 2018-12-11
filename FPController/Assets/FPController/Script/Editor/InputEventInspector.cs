using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FPController.FPEditor
{
    /// <summary>
    /// Class to store data about reordable list elements.
    /// </summary>
    public class ListItem
    {
        /// <summary>
        /// Elements height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Is the element expanded.
        /// </summary>
        public bool Expanded;

        public ListItem() : this(EditorGUIUtility.singleLineHeight, false)
        {
        }

        public ListItem(float _height, bool _expanded)
        {
            Height = _height;
            Expanded = _expanded;
        }
    }

    /// <summary>
    /// Custom inspector for input event.
    /// Primary function is to provide better list for InputEvent.m_events.
    /// </summary>
    [CustomEditor(typeof(InputEvent))]
    public class InputEventInspector : Editor
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Reordable list for InputEvent.m_events.
        /// </summary>
        private ReorderableList m_list;

        /// <summary>
        /// List of m_list element properties.
        /// Used to control list elements height (selected/not selected).
        /// </summary>
        private List<ListItem> m_heights;

        /*
         * Editor Functions.
         */

        private void OnEnable()
        {
            //Reset current list height properties.
            m_heights = new List<ListItem>();
            for(int i = 0; i < Target.EventCount; i++)
            {
                m_heights.Add(new ListItem());
            }

            //Create reordable list.
            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_events"), false, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => DrawHeaderCallback(rect),
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)
                    => DrawElementCallback(rect, index, isActive, isFocused),
                elementHeightCallback = (index) => ElementHeightCallback(index),
                onAddCallback = (list) => OnAddCallback(list),
                onRemoveCallback = (list) => OnRemoveCallback(list)
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_manager"));
            m_list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        /*
         * Reorderable List Functions.
         */

        private void DrawHeaderCallback(Rect _rect)
        {
            EditorGUI.LabelField(_rect, "Events");
        }

        private void DrawElementCallback(Rect _rect, int _index, bool _isActive, bool _isFocused)
        {
            var element = m_list.serializedProperty.GetArrayElementAtIndex(_index);
            DrawListItemHeader(_rect, element);
            //Draw content if selected.
            if(m_heights[_index].Expanded = _isActive)
                DrawListItemContent(_rect, _index, element);
        }

        private float ElementHeightCallback(int _index)
        {
            Repaint();
            return m_heights[_index].Expanded ? m_heights[_index].Height : EditorGUIUtility.singleLineHeight;
        }

        private void OnAddCallback(ReorderableList _list)
        {
            var index = _list.serializedProperty.arraySize;
            _list.serializedProperty.arraySize++;
            _list.index = index;
            var element = _list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Name").stringValue = "Input Name";
            element.FindPropertyRelative("KeyCode").enumValueIndex = 0;
            element.FindPropertyRelative("CombinationKeyCode").enumValueIndex = 0;
            element.FindPropertyRelative("KeyDownEvent").boolValue = false;
            element.FindPropertyRelative("KeyEvent").boolValue = false;
            element.FindPropertyRelative("KeyUpEvent").boolValue = false;
            //TODO: Reset unity events, currently cloning events from previous event.
            m_heights.Add(new ListItem());
        }

        private void OnRemoveCallback(ReorderableList _list)
        {
            m_heights.RemoveAt(_list.index);
            _list.serializedProperty.DeleteArrayElementAtIndex(_list.index);
        }

        /*
         * Private Functions.
         */

        /// <summary>
        /// Draw reordable list element header inside given rect with given property.
        /// </summary>
        /// <param name="_rect">Position inside reordable list.</param>
        /// <param name="_element">Headers property.</param>
        private void DrawListItemHeader(Rect _rect, SerializedProperty _element)
        {
            //Draw events name as a header.
            var rect = new Rect(_rect.x, _rect.y, _rect.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, _element.FindPropertyRelative("Name").stringValue);
            //Draw events key code
            var keyNames = _element.FindPropertyRelative("KeyCode").enumDisplayNames;
            var main = _element.FindPropertyRelative("KeyCode").enumValueIndex;
            var secondary = (KeyCode)_element.FindPropertyRelative("CombinationKeyCode").enumValueIndex;
            var keyCodeText = string.Format("{0}{1}",
                secondary != KeyCode.None ? string.Format("{0} + ", keyNames[(int)secondary]) : string.Empty,
                keyNames[main]);
            rect.x += (_rect.width / 2);
            EditorGUI.LabelField(rect, keyCodeText);
        }

        /// <summary>
        /// Draw propertys content inside reordable list.
        /// </summary>
        /// <param name="_rect">Position inside reordable list.</param>
        /// <param name="_index">Index for current element. Used to control height.</param>
        /// <param name="_element">Current property displayed.</param>
        private void DrawListItemContent(Rect _rect, int _index, SerializedProperty _element)
        {
            if(_rect.width < 0)
                return;

            var lineHeight = EditorGUIUtility.singleLineHeight * 1.1f;
            var rect = new Rect(_rect.x, _rect.y + lineHeight, _rect.width, lineHeight);
            //If single line, only draw one property on single line.
            var singleLine = _rect.width < 600f;
            var height = lineHeight * 2;
            EditorGUIUtility.labelWidth = _rect.width / (singleLine ? 2 : 4);
            EditorGUI.PropertyField(rect, _element.FindPropertyRelative("Name"), new GUIContent("Event Name"));
            //List of arrays, where single array represents single line in inspector, if inspector is wide enought.
            //Used to simplify calculating position and size for each field, since EditorGUIUtility is not supported inside reordable list.
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

            //Draw properties with a loop.
            foreach(var line in properties)
            {
                //Move to starting position.
                rect.x = _rect.x;
                rect.y += singleLine ? 0 : lineHeight;
                height += singleLine ? 0 : lineHeight;
                //Calculate width for each field, if the line contains multiple fields.
                rect.width = singleLine ? _rect.width : _rect.width / line.Length;
                //Draw properties on currentline.
                foreach(var entry in line)
                {
                    //Move the new line, if one line contains only one field.
                    rect.y += singleLine ? lineHeight : 0;
                    height += singleLine ? lineHeight : 0;
                    //Draw property field.
                    EditorGUI.PropertyField(rect, entry);
                    //Move position horizontally, if one line contains multiple fields.
                    rect.x += singleLine ? 0 : rect.width;
                }
            }

            //Move rect to the bottom left of the reordable list.
            rect.x = _rect.x;
            rect.width = _rect.width;
            rect.y += lineHeight;

            //Create dictionary containing events and boolean value telling if the event is wanted.
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

            //Loop through events.
            foreach(var entry in events)
            {
                //Draw event only if the user has selected to include it.
                if(entry.Value)
                {
                    //Draw event.
                    EditorGUI.PropertyField(rect, entry.Key);
                    //Calculate height for current event.
                    //Height can vary depending on how many functions are added to the event.
                    var addition = EditorGUI.GetPropertyHeight(entry.Key) + 15f;
                    height += addition;
                    rect.y += addition;
                }
            }

            //Set height for current property.
            m_heights[_index].Height = height;
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// Current inspector target as input event.
        /// </summary>
        private InputEvent Target
        {
            get
            {
                return (InputEvent)target;
            }
        }
    }
}