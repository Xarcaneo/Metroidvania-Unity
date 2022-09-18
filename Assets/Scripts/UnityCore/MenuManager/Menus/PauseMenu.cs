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
            Time.timeScale = 0;
            GameManager.Instance.isPaused = true;
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
            MainMenu.Open();
        }

        public override void OnReturnInput()
        {
            if (menuInput.actions["Return"].triggered) OnResumePressed();
        }

        public void OnResumePressed()
        {
            Time.timeScale = 1;
            GameManager.Instance.isPaused = false;
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
            GameManager.Instance.isPaused = false;
        }
    }
}