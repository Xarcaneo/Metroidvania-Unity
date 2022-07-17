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

        [SerializeField]
        private float _playDelay = 0.5f;

        [SerializeField]
        private TransitionFader startTransitionPrefab;

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
            StartCoroutine(OnMainMenuPressedRoutine());
            Time.timeScale = 1;
        }

        private IEnumerator OnMainMenuPressedRoutine()
        {
            TransitionFader.PlayTransition(startTransitionPrefab);
            yield return new WaitForSeconds(_playDelay);
            SceneManager.LoadScene(mainMenuIndex);
            MainMenu.Open();
        }
    }
}