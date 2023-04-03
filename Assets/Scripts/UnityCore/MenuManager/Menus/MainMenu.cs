using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;

namespace Menu
{
    public class MainMenu : Menu<MainMenu>
    { 
        public override void OnOpenMenu()
        {
            Time.timeScale = 1;
        }

        public override void OnReturnInput()
        {
            return;
        }

        public void OnPlayPressed()
        {
            PlayMenu.Open();
        }

        public void OnSettingPressed()
        {
            SettingsMenu.Open();
        }

        public void OnCreditsPressed()
        {
            CreditsScreen.Open();
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}