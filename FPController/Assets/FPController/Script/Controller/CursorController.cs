using UnityEngine;

namespace Mouse
{
    /// <summary>
    /// Controls cursors visibility during game.
    /// </summary>
    public class CursorController : MonoBehaviour
    {
        /*
         * Mono Behaviour Functions.
         */

        private void Start()
        {
            //Hide cursor when the game start.
            HideCursor();
        }

        private void Update()
        {
            switch(Cursor.lockState)
            {
                case CursorLockMode.None:
                    //Hide cursor if it's visible and user clicks with left mouse button.
                    if(Input.GetMouseButton(0))
                        HideCursor();
                    break;

                case CursorLockMode.Confined:
                case CursorLockMode.Locked:
                    //Display cursor if it's locked and user presses escape key.
                    if(Input.GetKeyDown(KeyCode.Escape))
                        ShowCursor();
                    break;
            }
        }

#if UNITY_EDITOR

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 300, 60), "Hide Cursor: Left Click\nShow Cursor: Esc");
        }

#endif

        /*
         * Static Functions.
         */

        /// <summary>
        /// Changes cursors visibility to given state.
        /// </summary>
        /// <param name="_visible">Display cursor.</param>
        public static void DisplayCursor(bool _visible)
        {
            if(_visible)
            {
                ShowCursor();
            }
            else
            {
                HideCursor();
            }
        }

        /// <summary>
        /// Changes cursors lock state to 'Locked' to hide cursor.
        /// </summary>
        public static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Changes cursors lock state to 'None' to display cursor.
        /// </summary>
        public static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}