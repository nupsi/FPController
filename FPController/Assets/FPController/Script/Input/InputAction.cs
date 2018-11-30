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
        private Action m_keyEvent;
        private Action m_keyUpEvent;
        private KeyCode m_combinationKey;

        /*
         * Public Functions.
         */

        public InputAction(KeyCode _key, Action _downEvent, Action _upEvent) : this(_key, _downEvent, _upEvent, KeyCode.None)
        {
        }

        public InputAction(KeyCode _key, Action _downEvent, Action _upEvent, KeyCode _combination)
        {
            KeyCode = _key;
            m_keyDownEvent = _downEvent;
            m_keyUpEvent = _upEvent;
            m_combinationKey = _combination;
        }

        public InputAction(KeyCode _key, Action _event) : this(_key, _event, KeyCode.None)
        {
        }

        public InputAction(KeyCode _key, Action _event, KeyCode _combination)
        {
            KeyCode = _key;
            m_keyEvent = _event;
            m_combinationKey = _combination;
        }

        public void KeyDown()
        {
            if(m_keyDownEvent != null && CombinationKeyDown)
            {
                m_keyDownEvent();
            }
        }

        public void Key()
        {
            if(m_keyEvent != null && CombinationKeyDown)
            {
                m_keyEvent();
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

        private bool CombinationKeyDown
        {
            get
            {
                return m_combinationKey == KeyCode.None
                    ? true
                    : Input.GetKey(m_combinationKey);
            }
        }
    }
}