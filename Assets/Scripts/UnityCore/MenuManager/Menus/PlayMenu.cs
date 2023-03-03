using Audio;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class PlayMenu : Menu<PlayMenu>
    {
        [SerializeField] SfxClip menuClip = default;
        public List<SaveSlot> saveSlots;

        public override void OnOpenMenu()
        {
            GameManager.Instance.currentSaveSlot = 1;
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
            GameMenu.Open();
        }

        public void OnSlotPressed()
        {
            InputManager.Instance.isInputActive = false;
            int active_slot = GameManager.Instance.currentSaveSlot;
            EventSystem.current.SetSelectedGameObject(null, null);
            menuClip.AudioGroup.RaiseStopAudioEvent(menuClip.AudioGroup.AudioSource);

            CustomActiveSaver.IsLoadingSavedGame = true;
            SaveSystem.loadEnded += OnLoadEnded;

            if (SaveSystem.HasSavedGameInSlot(active_slot))
            {
                SaveSystem.LoadFromSlot(active_slot);
            }
            else
            {
                SaveSystem.RestartGame("Area 0");
            }
        }

        public override void OnPlayerDeleteInput()
        {
            int active_slot = GameManager.Instance.currentSaveSlot;
            SaveSystem.DeleteSavedGameInSlot(active_slot);
            saveSlots[active_slot - 1].SetButtonContent();
        }

        void OnLoadEnded()
        {
            SaveSystem.loadEnded -= OnLoadEnded;
            CustomActiveSaver.IsLoadingSavedGame = false;
        }
    }
}