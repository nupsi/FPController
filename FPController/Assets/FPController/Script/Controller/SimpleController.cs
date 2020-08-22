using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Simplified first person controller containing input management.
    /// </summary>
    public class SimpleController : FirstPersonController
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Mouse look speed sensitivity.
        /// </summary>
        [SerializeField]
        private float m_lookSpeed = 2;

        /// <summary>
        /// Key to crouch down.
        /// </summary>
        [SerializeField]
        private KeyCode m_crouchKey = KeyCode.LeftControl;

        /// <summary>
        /// Key to run.
        /// </summary>
        [SerializeField]
        private KeyCode m_runKey = KeyCode.LeftShift;

        /// <summary>
        /// Key to jump.
        /// </summary>
        [SerializeField]
        private KeyCode m_jumpKey = KeyCode.Space;

        /*
         * Mono Behaviour Functions.
         */

        protected new void Update()
        {
            base.Update();
            MouseMove(MouseHorizontal, MouseVertical);
            UpdateInput();
        }

        public new void FixedUpdate()
        {
            base.FixedUpdate();
            Move(Horizontal, Vertical);
        }

        private void UpdateInput()
        {
            if(Input.GetKeyDown(m_crouchKey))
            {
                CrouchDown();
            }
            else if(Input.GetKeyUp(m_crouchKey))
            {
                CrouchUp();
            }

            if(Input.GetKeyDown(m_runKey))
            {
                StartRunning();
            }
            else if(Input.GetKeyUp(m_runKey))
            {
                StopRunning();
            }

            if(Input.GetKeyDown(m_jumpKey))
            {
                Jump();
            }
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