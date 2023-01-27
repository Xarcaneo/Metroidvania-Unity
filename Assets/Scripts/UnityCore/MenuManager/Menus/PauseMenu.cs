using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using PixelCrushers;
using Audio;
using UnityEngine.EventSystems;

namespace Menu
{
    public class PauseMenu : Menu<PauseMenu>
    {
        [SerializeField] private SfxClip sfxClip;

        public override void OnOpenMenu()
        {
            Time.timeScale = 0;
            sfxClip.AudioGroup.RaisePauseAudioEvent(sfxClip.AudioGroup.AudioSource);
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
            sfxClip.AudioGroup.RaiseUnPauseAudioEvent(sfxClip.AudioGroup.AudioSource);
            GameEvents.Instance.PauseTrigger(false);
            base.OnBackPressed();
        }

        public void OnSettingsPressed()
        {
            SettingsMenu.Open();
        }

        public void OnMainMenuPressed()
        {
            if (Player.Instance.GetComponent<PlayerPositionSaver>().m_lastCheckpointPosition != new Vector3(0, 0, 0))
                SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);

            InputManager.Instance.isInputActive = false;
            EventSystem.current.SetSelectedGameObject(null, null);
            AudioManager.Instance.gameObject.SetActive(false);
            AudioManager.Instance.Jukebox.gameObject.SetActive(false);
            SaveSystem.LoadScene("MainMenu");
            GameEvents.Instance.PauseTrigger(true);
            AudioManager.Instance.gameObject.SetActive(true);
        }
    }
}