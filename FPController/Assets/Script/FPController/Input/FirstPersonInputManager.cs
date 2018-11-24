using UnityEngine;

namespace FPController
{
    [RequireComponent(typeof(FirstPersonController))]
    public class FirstPersonInputManager : MonoBehaviour
    {
        /*
         * Variables.
         */

        private FirstPersonController m_controller;
        private KeyCode m_jumpKey = KeyCode.Space;
        private KeyCode m_crouchKey = KeyCode.LeftControl;
        private KeyCode m_runKey = KeyCode.LeftShift;
        private InputAction[] m_events;
        private float m_lookSpeed = 2;

        /*
         * MonoBehaviour Functions.
         */

        private void Awake()
        {
            m_controller = GetComponent<FirstPersonController>();
            m_events = new InputAction[]
            {
                new InputAction(m_jumpKey, m_controller.Jump, null),
                new InputAction(m_crouchKey, m_controller.CrouchDown, m_controller.CrouchUp),
                new InputAction(m_runKey, m_controller.StartRunning, m_controller.StopRunning)
            };
        }

        private void Update()
        {
            UpdateInput();
        }

        private void FixedUpdate()
        {
            FixedUpdateInput();
        }

        /*
         * Private Functions.
         */

        private void UpdateInput()
        {
            m_controller.MouseMove(MouseHorizontal, MouseVertical);
            foreach(var inputEvent in m_events)
            {
                if(Input.GetKeyDown(inputEvent.KeyCode))
                {
                    inputEvent.KeyDown();
                }
                else if(Input.GetKeyUp(inputEvent.KeyCode))
                {
                    inputEvent.KeyUp();
                }
            }
        }

        private void FixedUpdateInput()
        {
            m_controller.Move(Horizontal, Vertical);
        }

        /*
         * Accessors.
         */

        private float MouseHorizontal
        {
            get
            {
                return Input.GetAxis("Mouse X") * m_lookSpeed;
            }
        }

        private float MouseVertical
        {
            get
            {
                return Input.GetAxis("Mouse Y") * m_lookSpeed;
            }
        }

        private float Horizontal
        {
            get
            {
                return Input.GetAxis("Horizontal");
            }
        }

        private float Vertical
        {
            get
            {
                return Input.GetAxis("Vertical");
            }
        }
    }
}