using System;
using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Input Action to store two actions to be called on KeyDown() and KeyUp() when KeyCode is Up/Down.
    /// <see cref="KeyDown"/>
    /// <see cref="KeyUp"/>
    /// <see cref="KeyCode"/>
    /// </summary>
    public class InputAction
    {
        /*
         * Variables.
         */

        private Action m_keyDownEvent;
        private Action m_keyUpEvent;

        /*
         * Public Functions.
         */

        public InputAction(KeyCode _key, Action _downEvent, Action _upEvent)
        {
            KeyCode = _key;
            m_keyDownEvent = _downEvent;
            m_keyUpEvent = _upEvent;
        }

        public void KeyDown()
        {
            if(m_keyDownEvent != null)
            {
                m_keyDownEvent();
            }
        }

        public void KeyUp()
        {
            if(m_keyUpEvent != null)
            {
                m_keyUpEvent();
            }
        }

        /*
         * Accessors.
         */

        public KeyCode KeyCode { get; private set; }
    }
}