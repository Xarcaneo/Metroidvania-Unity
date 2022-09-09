using Audio;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class PlayMenu : Menu<PlayMenu>
    {
        [SerializeField] SfxClip menuClip = default;

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
            int active_slot = GameManager.Instance.currentSaveSlot;
            EventSystem.current.SetSelectedGameObject(null, null);
            menuClip.AudioGroup.RaiseStopAudioEvent(menuClip.AudioGroup.AudioSource);

            if (SaveSystem.HasSavedGameInSlot(active_slot))
            {
                SaveSystem.LoadFromSlot(active_slot);
            }
            else
            {
                SaveSystem.RestartGame("Area 0");
            }
        }

        public void OnDeletePressed()
        {
            int active_slot = GameManager.Instance.currentSaveSlot;
            SaveSystem.DeleteSavedGameInSlot(active_slot);
        }
    }
}