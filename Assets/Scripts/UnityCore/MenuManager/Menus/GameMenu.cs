using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class GameMenu : Menu<GameMenu>
    {
        public override void OnReturnInput(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                OnPausePressed();
            }
        }

        public void OnPausePressed()
        {
            Time.timeScale = 0;

            PauseMenu.Open();
        }
    }
}