using System;
using UnityEngine;
using UnityEngine.Events;

namespace FPController
{
    /// <summary>
    /// Exposing Unity Event to be shown in inspector.
    /// </summary>
    [Serializable]
    public class CustomEvent : UnityEvent { }

    /// <summary>
    /// Class to store input event data.
    /// Contains exposed Unity Events where user can add functions inside Unity Editor.
    /// </summary>
    [Serializable]
    public class InputEventData
    {
        /// <summary>
        /// Event name inside editor.
        /// </summary>
        public string Name = "Input Event";

        /// <summary>
        /// Primary Key Code expected on key event.
        /// </summary>
        public KeyCode KeyCode;

        /// <summary>
        /// Combination key code.
        /// Expected to be pressed down before primary key, if assigned.
        /// </summary>
        public KeyCode CombinationKeyCode = KeyCode.None;

        /// <summary>
        /// Is get key down event called.
        /// </summary>
        public bool KeyDownEvent = true;

        /// <summary>
        /// Is get key event called.
        /// </summary>
        public bool KeyEvent = false;

        /// <summary>
        /// Is get key up called.
        /// </summary>
        public bool KeyUpEvent = true;

        /// <summary>
        /// Events called on get key down.
        /// </summary>
        public CustomEvent GetKeyDown;

        //Events called on get key.
        public CustomEvent GetKey;

        /// <summary>
        /// Events called on get key up.
        /// </summary>
        public CustomEvent GetKeyUp;
    }
}