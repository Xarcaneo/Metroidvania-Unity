using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using PixelCrushers;

namespace Menu
{
    public class MainMenu : Menu<MainMenu>
    {
        [SerializeField] SfxClip menuClip = default;

        public override void OnOpenMenu()
        {
            Time.timeScale = 1;
            menuClip.AudioGroup.RaiseFadeInAudioEvent(menuClip.AudioGroup.AudioSource, menuClip, menuClip.AudioConfiguration);
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