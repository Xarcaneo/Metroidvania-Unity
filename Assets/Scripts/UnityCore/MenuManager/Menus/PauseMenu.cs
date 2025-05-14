using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using PixelCrushers;
using UnityEngine.EventSystems;

namespace Menu
{
    /// <summary>
    /// Handles the pause menu functionality, including pausing the game, navigating to settings, and returning to the main menu.
    /// </summary>
    public class PauseMenu : Menu<PauseMenu>
    {
        /// <summary>
        /// Called when the pause menu is opened.
        /// Sets the game to a paused state and registers event handlers.
        /// </summary>
        public override void OnOpenMenu()
        {
            SetPauseState(true);
        }

        /// <summary>
        /// Called when the component is enabled.
        /// Registers the scene loaded event handler.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Called when the component is disabled.
        /// Unregisters the scene loaded event handler.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            SaveSystem.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Handles the scene loaded event.
        /// Opens the main menu after the scene is loaded.
        /// </summary>
        /// <param name="sceneName">The name of the loaded scene.</param>
        /// <param name="sceneIndex">The index of the loaded scene.</param>
        private void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            MainMenu.Open();
        }

        /// <summary>
        /// Handles the return input to resume the game.
        /// </summary>
        public override void OnReturnInput() => OnResumePressed();
        
        /// <summary>
        /// Handles the pause input to resume the game when the pause menu is already open.
        /// </summary>
        public override void OnPauseInput() => OnResumePressed();

        /// <summary>
        /// Resumes the game by unpausing and closing the menu.
        /// </summary>
        public void OnResumePressed()
        {
            SetPauseState(false);
            base.OnBackPressed();
        }

        /// <summary>
        /// Opens the settings menu from the pause menu.
        /// </summary>
        public void OnSettingsPressed()
        {
            SettingsMenu.Open();
        }

        /// <summary>
        /// Returns to the main menu from the pause menu.
        /// </summary>
        public void OnMainMenuPressed()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.isInputActive = false;
            }

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null, null);
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.ClearSFXBus();
            }

            SaveSystem.LoadScene("MainMenu");
            GameEvents.Instance.PauseTrigger(true);
            GameEvents.Instance.EndSession();
        }

        /// <summary>
        /// Sets the game's pause state and triggers the corresponding game event.
        /// </summary>
        /// <param name="isPaused">True to pause the game, false to resume.</param>
        private void SetPauseState(bool isPaused)
        {
            Time.timeScale = isPaused ? 0 : 1;
            GameEvents.Instance.PauseTrigger(isPaused);
        }
    }
}
