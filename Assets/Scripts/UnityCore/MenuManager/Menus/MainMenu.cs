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
            menuClip.AudioGroup.RaiseFadeInAudioEvent(menuClip.AudioGroup.AudioSource, menuClip, menuClip.AudioConfiguration);
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        public void OnPlayPressed()
        {
            //menuClip.AudioGroup.RaiseStopAudioEvent(menuClip.AudioGroup.AudioSource);
            PlayMenu.Open();
            //SaveSystem.LoadScene("Area 0@Spawn");
        }

        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
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