using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using PixelCrushers;
using UnityEngine.EventSystems;

namespace Menu
{
    public class PauseMenu : Menu<PauseMenu>
    {
        public override void OnOpenMenu()
        {
            Time.timeScale = 0;
            GameEvents.Instance.PauseTrigger(true);
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
            MainMenu.Open();
        }

        public override void OnReturnInput()
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
            OnResumePressed();
        }

        public void OnResumePressed()
        {
            Time.timeScale = 1;
            GameEvents.Instance.PauseTrigger(false);
            base.OnBackPressed();
        }

        public void OnSettingsPressed()
        {
            SettingsMenu.Open();
        }

        public void OnMainMenuPressed()
        {
            InputManager.Instance.isInputActive = false;
            EventSystem.current.SetSelectedGameObject(null, null);
            SaveSystem.LoadScene("MainMenu");
            GameEvents.Instance.PauseTrigger(true);
        }
    }
}