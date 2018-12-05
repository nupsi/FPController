using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FPController
{
    public class InputEvent : MonoBehaviour
    {
        /*
         * Custom classes.
         */

        [Serializable]
        private class CustomEvent : UnityEvent { }

        [Serializable]
        private class Event
        {
            public string Name = "Input Event";
            public KeyCode KeyCode;
            public KeyCode CombinationKeyCode = KeyCode.None;
            public bool KeyDownEvent = true;
            public bool KeyEvent = false;
            public bool KeyUpEvent = true;
            public CustomEvent GetKeyDown;
            public CustomEvent GetKey;
            public CustomEvent GetKeyUp;
        }

        /*
         * Variables.
         */

        [SerializeField]
        private InputManager m_manager;

        [SerializeField]
        private List<Event> m_events;

        /*
         * Mono Behaviour Functions.
         */

        private void Start()
        {
            m_manager.Register(Actions);
        }

        /*
         * Accessors.
         */

        private List<InputAction> Actions
        {
            get
            {
                var actions = new List<InputAction>();
                foreach(var current in m_events)
                {
                    if(current.KeyCode != KeyCode.None)
                    {
                        var down = current.KeyDownEvent ? current.GetKeyDown.Invoke : new Action(() => { });
                        var key = current.KeyEvent ? current.GetKey.Invoke : new Action(() => { });
                        var up = current.KeyUpEvent ? current.GetKeyUp.Invoke : new Action(() => { });
                        actions.Add(new InputAction(current.KeyCode, down, key, up, current.CombinationKeyCode));
                    }
                    else
                    {
                        Debug.Log("Input Event without key code!");
                    }
                }
                return actions;
            }
        }

        /*
         * Accessors.
         */

        public int EventCount
        {
            get
            {
                return m_events.Count;
            }
        }
    }
}