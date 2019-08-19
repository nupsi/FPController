using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Sciptable keybind to set multiple key codes for single action.
    /// Assign valid keys to KeyCodes array and all the required compination keys to CombinationKeyCodes.
    /// One KeyCodes key code and all combination key codes are required to return true on key events.
    /// </summary>
    [CreateAssetMenu(fileName = "Keybind", menuName = "InputManager/Keybind", order = 1100)]
    public class ScriptableKeybind : ScriptableObject
    {
        /// <summary>
        /// Keybind Display name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Primary key codes.
        /// </summary>
        public KeyCode[] KeyCodes;

        /// <summary>
        /// Combination key codes.
        /// </summary>
        public KeyCode[] CombinationKeyCodes;

        /// <summary>
        /// Does this scriptable keybind contain any key codes.
        /// </summary>
        public bool HasKeyCode
        {
            get
            {
                return KeyCodes.Length > 0;
            }
        }

        public bool GetKeyDown()
        {
            foreach(var code in KeyCodes)
            {
                if(code != KeyCode.None && Input.GetKeyDown(code) && CombinationDown)
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetKey()
        {
            foreach(var code in KeyCodes)
            {
                if(code != KeyCode.None && Input.GetKey(code) && CombinationDown)
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetKeyUp()
        {
            foreach(var code in KeyCodes)
            {
                if(code != KeyCode.None && Input.GetKeyUp(code) && CombinationDown)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Are all the combination keys down.
        /// </summary>
        private bool CombinationDown
        {
            get
            {
                var isDown = true;
                if(CombinationKeyCodes.Length > 0)
                {
                    foreach(var code in CombinationKeyCodes)
                    {
                        if(code != KeyCode.None && !Input.GetKey(code))
                        {
                            isDown = false;
                        }
                    }
                }
                return isDown;
            }
        }
    }
}