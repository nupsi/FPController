using System;
using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Stores key code(s) and reference(s) to function(s).
    /// Allows simple way to check if key is used and call stored function.
    /// <example>
    /// //Define array of differend action.
    /// var actions = InputAction[] 
    /// { 
    ///     //Create input action for jump.
    ///     //controller.Jump() gets called when actions[0].KeyDown() is called.
    ///     new InputAction(KeyCode.Space, controller.Jump, null)
    /// };
    /// ...
    /// //Loop through the array of actions.
    /// foreach(var action in actions)
    /// {
    ///     //Check if key is down for current action.
    ///     if(Input.GetKeyDown(action.KeyCode))
    ///     {
    ///         //Call action.KeyDown() to execute function referenced by input action.
    ///         //For this example this would call controller.Jump() when space is pressed.
    ///         action.KeyDown();
    ///     }
    ///     //Check if pressed and up.
    /// }
    /// </example>
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

        public InputAction(KeyCode _key, Action _downEvent, Action _upEvent) 
            : this(_key, _downEvent, _upEvent, KeyCode.None) { }

        public InputAction(KeyCode _key, Action _downEvent, Action _upEvent, KeyCode _combination) 
            : this(_key, _downEvent, null, _upEvent, _combination) { }

        public InputAction(KeyCode _key, Action _event) 
            : this(_key, _event, KeyCode.None) { }

        public InputAction(KeyCode _key, Action _event, KeyCode _combination) 
            : this(_key, null, _event, null, _combination) { }

        public InputAction(KeyCode _key, Action _downEvent, Action _event, Action _upEvent, KeyCode _combination)
        {
            KeyCode = _key;
            m_keyEvent = _event;
            m_keyUpEvent = _upEvent;
            m_keyDownEvent = _downEvent;
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