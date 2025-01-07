using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using FMODUnity;

namespace Menu
{
    /// <summary>
    /// Represents the main menu of the game, handling navigation to other menus and managing game state.
    /// </summary>
    public class MainMenu : Menu<MainMenu>
    {
        /// <summary>
        /// The music event reference for the main menu background music.
        /// </summary>
        [field: Header("Music")]
        [field: SerializeField] public EventReference music { get; private set; }

        /// <summary>
        /// The title screen component to manage title screen transitions.
        /// </summary>
        [field: Header("Title Screen")]
        [SerializeField] private TitleScreen m_titleScreen;

        /// <summary>
        /// The parent GameObject that holds the menu buttons.
        /// </summary>
        [field: Header("Buttons")]
        [SerializeField] private GameObject m_buttons;

        /// <summary>
        /// Called when the main menu is opened.
        /// Manages game state and plays the background music.
        /// </summary>
        public override void OnOpenMenu()
        {
            if (m_titleScreen != null && m_titleScreen.transitionFinished)
            {
                m_buttons.SetActive(true);

                Time.timeScale = 1;
                GameEvents.Instance.PauseTrigger(false);

                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayMusic(music);
                }
                else
                {
                    Debug.LogError("AudioManager.instance is null.");
                }

                if (m_buttons != null)
                {
                    m_buttons.SetActive(true);
                }
                else
                {
                    Debug.LogError("Buttons GameObject is not assigned.");
                }
            }
        }

        /// <summary>
        /// Handles the return input event.
        /// Currently, this method is not implemented.
        /// </summary>
        public override void OnReturnInput()
        {
            return;
        }

        /// <summary>
        /// Opens the play menu when the play button is pressed.
        /// </summary>
        public void OnPlayPressed()
        {
            PlayMenu.Open();
        }

        /// <summary>
        /// Opens the settings menu when the settings button is pressed.
        /// </summary>
        public void OnSettingPressed()
        {
            SettingsMenu.Open();
        }

        /// <summary>
        /// Opens the credits screen when the credits button is pressed.
        /// </summary>
        public void OnCreditsPressed()
        {
            CreditsScreen.Open();
        }

        /// <summary>
        /// Exits the application when the back button is pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}
