using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class GameSettingsMenu : Menu<GameSettingsMenu>
    {
        public void OnControlsPressed()
        {
            ControlsMenu.Open();

        }
        public override void OnBackPressed()
        {
            // or add extra logic here

            base.OnBackPressed();

            // add extra logic here
        }
    }
}