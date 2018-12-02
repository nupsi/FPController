using UnityEngine;

namespace FPController
{
    [RequireComponent(typeof(FirstPersonController))]
    public class FirstPersonInputManager : MonoBehaviour
    {
        private FirstPersonController m_controller;
        private float m_lookSpeed = 2;

        private void Awake()
        {
            m_controller = GetComponent<FirstPersonController>();
        }

        private void Update()
        {
            m_controller.MouseMove(MouseHorizontal, MouseVertical);
        }

        private void FixedUpdate()
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