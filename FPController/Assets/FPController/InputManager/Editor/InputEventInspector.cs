using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FPController.FPEditor
{
    /// <summary>
    /// Custom inspector for input event.
    /// Primary function is to provide better list for InputEvent.m_events.
    /// </summary>
    [CustomEditor(typeof(InputEvent))]
    public class InputEventInspector : Editor
    {
        /// <summary>
        /// Keep track of the expanded input events.
        /// </summary>
        private List<bool> m_expanded;

        protected void OnEnable()
        {
            m_expanded = new List<bool>();
            foreach(var key in Target.Events)
            {
                m_expanded.Add(false);
            }
        }

        public override void OnInspectorGUI()
        {
            //Color for the second level of box layout.
            var color = new Color(0.9f, 0.9f, 0.9f);
            //Position for the current element.
            //Used to get a value from the serialized property.
            var count = 0;
            //Current target property.
            var targetProperty = serializedObject.FindProperty("m_events");

            //Target input manager.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_manager"));
            GUILayout.Label("Input Events", EditorStyles.boldLabel);

            //Loop through the current events.
            foreach(var target in Target.Events)
            {
                //Get the current child property from the target property as serialized property.
                //Used to render unity event property field.
                var property = targetProperty.GetArrayElementAtIndex(count);
                //Current event title.
                var text = "Input Event: " + (target.Keybind == null ? "None" : "'" + target.Keybind.Name + "'");
                //TODO: Make foldout the first level box layout.
                m_expanded[count] = EditorGUILayout.Foldout(m_expanded[count], text);
                if(m_expanded[count])
                {
                    GUILayout.BeginVertical("Box");
                    EditorGUILayout.Space();

                    //Object field for a scripptable keybind.
                    target.Keybind = EditorGUILayout.ObjectField("Scriptable Keybind", target.Keybind, typeof(ScriptableKeybind), false) as ScriptableKeybind;

                    //Toggle and property field for key down event.
                    StartBox(color);
                    target.KeyDownEvent = EditorGUILayout.Toggle("Get Key Down Event", target.KeyDownEvent);
                    if(target.KeyDownEvent)
                    {
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("GetKeyDown"));
                    }
                    EditorGUILayout.EndVertical();

                    //Toggle and property field for key event.
                    StartBox(color);
                    target.KeyEvent = EditorGUILayout.Toggle("Get Key Event", target.KeyEvent);
                    if(target.KeyEvent)
                    {
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("GetKey"));
                    }
                    EditorGUILayout.EndVertical();

                    //Toggle and property field for key up event.
                    StartBox(color);
                    target.KeyUpEvent = EditorGUILayout.Toggle("Get Key Up Event", target.KeyUpEvent);
                    if(target.KeyUpEvent)
                    {
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("GetKeyUp"));
                    }
                    EditorGUILayout.EndVertical();

                    //Remove button to remove the current element from the target.
                    if(GUILayout.Button("Remove"))
                    {
                        m_expanded.RemoveAt(count);
                        Target.Events.Remove(target);
                        Repaint();
                        return;
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }
                count++;
            }

            //Button to add a input event to the current target.
            if(GUILayout.Button("Add Input Event"))
            {
                m_expanded.Add(true);
                Target.Events.Add(new InputEventData());
            }

            //Apply modified properties to the target object.
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw a EditorGUILayout.BeginVertical("Box") with a given color while retaining the original color.
        /// </summary>
        /// <param name="color">GUI color for the layout.</param>
        private void StartBox(Color color)
        {
            var original = GUI.color;
            GUI.color = color;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = original;
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