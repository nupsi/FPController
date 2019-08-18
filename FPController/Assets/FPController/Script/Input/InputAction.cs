﻿using System;
using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Stores key code(s) and reference(s) to function(s).
    /// Allows simple way to check if key is used and call stored function.
    /// </summary>
    /// <example>
    /// Example of using Input action to make a controller jump.
    /// <code>
    /// private InputAction[] actions;
    ///
    /// private void Awake()
    /// {
    ///     actions = new InputAction[]
    ///     {
    ///         new InputAction(KeyCode.Space, controller.Jump, null)
    ///     };
    /// }
    ///
    /// private void Update()
    /// {
    ///     foreach(var action in actions)
    ///     {
    ///         action.GetKeyDown();
    ///         action.GetKey();
    ///         action.GetKeyUp();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class InputAction : IKeyEvent
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

        public void GetKeyDown()
        {
            if(m_keyDownEvent != null)
            {
                if(Input.GetKeyDown(KeyCode) && CombinationKeyDown)
                {
                    m_keyDownEvent();
                }
            }
        }

        public void GetKey()
        {
            if(m_keyEvent != null)
            {
                if(Input.GetKey(KeyCode) && CombinationKeyDown)
                {
                    m_keyEvent();
                }
            }
        }

        public void GetKeyUp()
        {
            if(m_keyUpEvent != null)
            {
                if(Input.GetKeyUp(KeyCode))
                {
                    m_keyUpEvent();
                }
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
                return m_combinationKey == KeyCode.None || Input.GetKey(m_combinationKey);
            }
        }
    }
}