using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using FMODUnity;

namespace Menu
{
    public class MainMenu : Menu<MainMenu>
    {
        [field: Header("Music")]
        [field: SerializeField] public EventReference music { get; private set; }

        public override void OnOpenMenu()
        {
            Time.timeScale = 1;
            GameEvents.Instance.PauseTrigger(false);
            AudioManager.instance.PlayMusic(music);
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