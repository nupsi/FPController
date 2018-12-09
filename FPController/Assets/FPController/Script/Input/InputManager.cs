using System.Collections.Generic;
using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Contains list of actions called on get key down/up and while key is pressed.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        /*
         * Variables.
         */

        /// <summary>
        /// List of registered actions.
        /// </summary>
        private List<InputAction> m_actions;

        /*
         * MonoBehaviour Functions.
         */

        private void Awake()
        {
            m_actions = new List<InputAction>();
        }

        private void Update()
        {
            //Loop through registered actions.
            foreach(var inputEvent in m_actions)
            {
                inputEvent.GetKeyDown();
                inputEvent.GetKey();
                inputEvent.GetKeyUp();
            }
        }

        /*
         * Public Functions.
         */

        /// <summary>
        /// Add list of actions to input manager.
        /// </summary>
        /// <param name="_actions">Actions to add.</param>
        public void Register(List<InputAction> _actions)
        {
            m_actions.AddRange(_actions);
        }
    }
}