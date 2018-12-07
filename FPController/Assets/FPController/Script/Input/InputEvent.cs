using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPController
{
    public class InputEvent : MonoBehaviour
    {
        /*
         * Variables.
         */

        [SerializeField]
        private InputManager m_manager;

        [SerializeField]
        private List<InputEventData> m_events;

        /*
         * Mono Behaviour Functions.
         */

        private void Awake()
        {
            if(m_manager == null)
            {
                m_manager = gameObject.AddComponent<InputManager>();
            }
        }

        private void Start()
        {
            m_manager.Register(Actions);
        }

        private void Reset()
        {
            m_events = new List<InputEventData>();
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