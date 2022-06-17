using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Menu
{
    public class MainMenu : Menu<MainMenu>
    {
        public void OnPlayPressed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadLevel(1);
            }

            GameMenu.Open();
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