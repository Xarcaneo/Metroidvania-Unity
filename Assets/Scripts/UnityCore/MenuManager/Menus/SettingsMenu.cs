using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        public void OnGamePressed()
        {
            GameSettingsMenu.Open();
        }

        public override void OnBackPressed()
        {
            // or add extra logic here

            base.OnBackPressed();

            // add extra logic here
        }
    }
}