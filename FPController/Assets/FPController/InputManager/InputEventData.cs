using System;
using UnityEngine.Events;

namespace FPController
{
    /// <summary>
    /// Exposing Unity Event for the Inspector.
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
        public ScriptableKeybind Keybind;

        /// <summary>
        /// Is get key down event called.
        /// </summary>
        public bool KeyDownEvent;

        /// <summary>
        /// Is get key event called.
        /// </summary>
        public bool KeyEvent;

        /// <summary>
        /// Is get key up called.
        /// </summary>
        public bool KeyUpEvent;

        /// <summary>
        /// Events called on get key down.
        /// </summary>
        public CustomEvent GetKeyDown;

        /// <summary>
        /// Events called on get key.
        /// </summary>
        public CustomEvent GetKey;

        /// <summary>
        /// Events called on get key up.
        /// </summary>
        public CustomEvent GetKeyUp;
    }
}