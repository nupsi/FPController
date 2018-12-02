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
                        actions.Add(new InputAction(
                                current.KeyCode, 
                                current.GetKeyDown.Invoke,
                                current.GetKey.Invoke,
                                current.GetKeyUp.Invoke,
                                current.CombinationKeyCode));
                    }
                    else
                    {
                        Debug.Log("Input Event without key code!");
                    }
                }
                return actions;
            }
        }
    }
}