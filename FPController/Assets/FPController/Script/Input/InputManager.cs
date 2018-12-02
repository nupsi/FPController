using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPController
{
    public class InputManager : MonoBehaviour
    {
        /*
         * Variables.
         */

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

        public void Register(List<InputAction> _actions)
        {
            m_actions.AddRange(_actions);
        }
    }
}