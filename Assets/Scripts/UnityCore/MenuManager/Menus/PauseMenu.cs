using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using PixelCrushers;

namespace Menu
{
    public class PauseMenu : Menu<PauseMenu>
    {
        public override void OnOpenMenu()
        {
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
            MainMenu.Open();
        }

        public override void OnReturnInput(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                OnResumePressed();
            }
        }

        public void OnResumePressed()
        {
            Time.timeScale = 1;
            base.OnBackPressed();
        }

        public void OnSettingsPressed()
        {
            SettingsMenu.Open();
        }

        public void OnMainMenuPressed()
        {
            SaveSystem.LoadScene("MainMenu");
            Time.timeScale = 1;
        }
    }
}