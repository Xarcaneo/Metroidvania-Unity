using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class GameMenu : Menu<GameMenu>
    {
        public override void OnReturnInput()
        {
            if (menuInput.actions["Return"].triggered) OnPausePressed();
        }

        public void OnPausePressed()
        {
            PauseMenu.Open();
        }
    }
}