using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Allows user to bind public functions to key codes inside Unity Editor.
    /// </summary>
    public class InputEvent : MonoBehaviour
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Empty action to use when action is disabled in InputEventData.
        /// </summary>
        private static readonly Action None = new Action(() => { });

        /// <summary>
        /// Target input manager.
        /// Event data is converted to input actions and registered here.
        /// </summary>
        [SerializeField]
        private InputManager m_manager;

        /// <summary>
        /// List of stored input events.
        /// </summary>
        [SerializeField]
        private List<InputEventData> m_events;

        /*
         * Mono Behaviour Functions.
         */

        protected void Awake()
        {
            if(m_manager == null)
            {
                m_manager = gameObject.AddComponent<InputManager>();
            }
        }

        protected void Start()
        {
            //Register events by converting them into actions.
            m_manager.Register(Actions);
        }

        protected void Reset()
        {
            m_events = new List<InputEventData>();
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// Converst current list of event data into list of actions.
        /// </summary>
        private List<IKeyEvent> Actions
        {
            get
            {
                var actions = new List<IKeyEvent>();
                foreach(var current in m_events)
                {
                    if(current.Keybind.HasKeyCode)
                    {
                        var down = current.KeyDownEvent ? current.GetKeyDown.Invoke : None;
                        var key = current.KeyEvent ? current.GetKey.Invoke : None;
                        var up = current.KeyUpEvent ? current.GetKeyUp.Invoke : None;
                        actions.Add(new InputAction(current.Keybind, down, key, up));
                    }
                    else
                    {
                        Debug.Log("Input Event without key code!");
                    }
                }
                return actions;
            }
        }

        /// <summary>
        /// List of stored input events.
        /// </summary>
        public List<InputEventData> Events
        {
            get
            {
                return m_events;
            }
        }
    }
}