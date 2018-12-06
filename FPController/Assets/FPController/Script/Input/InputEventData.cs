using System;
using UnityEngine;
using UnityEngine.Events;

namespace FPController
{
    [Serializable]
    public class CustomEvent : UnityEvent { }

    [Serializable]
    public class InputEventData
    {
        public string Name = "Input Event";
        public KeyCode KeyCode;
        public KeyCode CombinationKeyCode = KeyCode.None;
        public bool KeyDownEvent = true;
        public bool KeyEvent = false;
        public bool KeyUpEvent = true;
        public CustomEvent GetKeyDown;
        public CustomEvent GetKey;
        public CustomEvent GetKeyUp;
    }
}