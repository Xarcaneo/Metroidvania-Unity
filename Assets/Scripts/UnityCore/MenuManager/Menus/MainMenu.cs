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
        }

        public void OnSettingPressed()
        {
            if (MenuManager.Instance != null && SettingsMenu.Instance != null)
            {
                MenuManager.Instance.OpenMenu(SettingsMenu.Instance);
            }
        }

        public void OnCreditsPressed()
        { 
            if (MenuManager.Instance != null && CreditsScreen.Instance != null)
            {
                MenuManager.Instance.OpenMenu(CreditsScreen.Instance);
            }
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}