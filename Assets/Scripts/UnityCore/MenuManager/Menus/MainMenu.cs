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

        [field: Header("Title Screen")]
        [SerializeField] private TitleScreen m_titleScreen;

        [field: Header("Buttons")]
        [SerializeField] private GameObject m_buttons;

        public override void OnOpenMenu()
        {
            if (m_titleScreen.transitionFinished)
            {
                Time.timeScale = 1;
                GameEvents.Instance.PauseTrigger(false);
                AudioManager.instance.PlayMusic(music);
                m_buttons.SetActive(true);
            }
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