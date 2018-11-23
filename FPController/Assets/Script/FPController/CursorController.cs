using UnityEngine;

namespace Mouse
{
    public class CursorController : MonoBehaviour
    {
        private void Start()
        {
            HideMouse();
        }

        private void Update()
        {
            switch(Cursor.lockState)
            {
                case CursorLockMode.None:
                    if(Input.GetMouseButton(0))
                        HideMouse();
                    break;

                case CursorLockMode.Locked:
                    if(Input.GetKeyDown(KeyCode.Escape))
                        ShowMouse();
                    break;
            }
        }

        public static void HideMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void ShowMouse()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}