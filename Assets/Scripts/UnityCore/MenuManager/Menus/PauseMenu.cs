using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Menu
{
    public class PauseMenu : Menu<PauseMenu>
    {
        [SerializeField]
        private int mainMenuIndex = 0;

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
            Time.timeScale = 1;
            SceneManager.LoadScene(mainMenuIndex);

            MainMenu.Open();
        }
    }
}