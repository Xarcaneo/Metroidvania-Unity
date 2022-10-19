using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class GameMenu : Menu<GameMenu>
    {
        public override void CustomUpdate()
        {
            base.CustomUpdate();
            OnPlayerMenuInput();
        }

        public override void OnReturnInput()
        {
            if (menuInput.actions["Return"].triggered) OnPausePressed();
        }

        private void OnPlayerMenuInput()
        {
            if (menuInput.actions["PlayerMenu"].triggered)
            {
                PlayerMenu.Open();
            }
        }

        public void OnPausePressed()
        {
            PauseMenu.Open();
        }
    }
}