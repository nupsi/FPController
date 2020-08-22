using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Input Manager for First Person Controller.
    /// Calls mouse and keyboard movement.
    /// </summary>
    [RequireComponent(typeof(FirstPersonController))]
    public class FirstPersonInputManager : MonoBehaviour
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Target controller.
        /// </summary>
        private FirstPersonController m_controller;

        /// <summary>
        /// Multiplier for mouse movement ('Sensitivity').
        /// </summary>
        private float m_lookSpeed = 2;

        /*
         * Mono Behaviour Functions.
         */

        protected void Awake()
        {
            m_controller = GetComponent<FirstPersonController>();
        }

        protected void Update()
        {
            //Update mouse movenent each frame.
            m_controller.MouseMove(MouseHorizontal, MouseVertical);
        }

        protected void FixedUpdate()
        {
            //Update movement every fixed update because we are using rigidbody and physics are updated on fixed update.
            m_controller.Move(Horizontal, Vertical);
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// Horizontal mouse movement multiplied by look speed.
        /// </summary>
        private float MouseHorizontal
        {
            get
            {
                return Input.GetAxis("Mouse X") * m_lookSpeed;
            }
        }

        /// <summary>
        /// Vertical mouse movement multiplied by look speed.
        /// </summary>
        private float MouseVertical
        {
            get
            {
                return Input.GetAxis("Mouse Y") * m_lookSpeed;
            }
        }

        /// <summary>
        /// Horizontal keyboard movement.
        /// Defined by unity Input.GetAxis("Horizontal") at Edit/Project Settings/Input.
        /// </summary>
        private float Horizontal
        {
            get
            {
                return Input.GetAxisRaw("Horizontal");
            }
        }

        /// <summary>
        /// Vertical keyboard movement.
        /// Defined by unity Input.GetAxis("Vertical") at Edit/Project Settings/Input.
        /// </summary>
        private float Vertical
        {
            get
            {
                return Input.GetAxisRaw("Vertical");
            }
        }
    }
}