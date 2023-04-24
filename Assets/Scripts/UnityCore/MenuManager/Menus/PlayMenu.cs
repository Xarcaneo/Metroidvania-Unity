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
            GameManager.Instance.UpdateBindingToDialogue();
            GameEvents.Instance.NewSession();
        }

        public void OnSlotPressed()
        {
            AudioManager.instance.StopMusic();

            InputManager.Instance.isInputActive = false;
            int active_slot = GameManager.Instance.currentSaveSlot;
            EventSystem.current.SetSelectedGameObject(null, null);

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
    }
}