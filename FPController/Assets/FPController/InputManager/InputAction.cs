using System;

namespace FPController
{
    /// <summary>
    /// Stores key code(s) and reference(s) to function(s).
    /// Allows simple way to check if key is used and call stored function.
    /// </summary>
    public class InputAction : IKeyEvent
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Delegate for actions called on GetKeyDown().
        /// </summary>
        private readonly Action m_keyDownEvent;

        /// <summary>
        /// Delegate for actions called on GetKey().
        /// </summary>
        private readonly Action m_keyEvent;

        /// <summary>
        /// Delegate for actions called on GetKeyUp().
        /// </summary>
        private readonly Action m_keyUpEvent;

        /*
         * Public Functions.
         */

        public InputAction(ScriptableKeybind _key, Action _downEvent, Action _upEvent)
            : this(_key, _downEvent, null, _upEvent) { }

        public InputAction(ScriptableKeybind _key, Action _event)
            : this(_key, null, _event, null) { }

        public InputAction(ScriptableKeybind _key, Action _downEvent, Action _event, Action _upEvent)
        {
            Keybind = _key;
            m_keyEvent = _event;
            m_keyUpEvent = _upEvent;
            m_keyDownEvent = _downEvent;
        }

        public void GetKeyDown()
        {
            if(m_keyDownEvent != null && Keybind.GetKeyDown())
            {
                m_keyDownEvent();
            }
        }

        public void GetKey()
        {
            if(m_keyEvent != null && Keybind.GetKey())
            {
                m_keyEvent();
            }
        }

        public void GetKeyUp()
        {
            if(m_keyUpEvent != null && Keybind.GetKeyUp())
            {
                m_keyUpEvent();
            }
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// Scriptable Keybind for the input action.
        /// </summary>
        public ScriptableKeybind Keybind { get; private set; }
    }
}