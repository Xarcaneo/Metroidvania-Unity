using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class PlayMenu : Menu<PlayMenu>
    {
        [SerializeField] private string m_startingArea = "Vestibule 0";
        [SerializeField] private List<SaveSlot> saveSlots;
        [SerializeField] private Sprite normalSprite; // Sprite for normal mode
        [SerializeField] private Sprite deleteSprite; // Sprite for delete mode
        [SerializeField] private GameObject deleteText;
        [SerializeField] private GameObject warningUI;
        [SerializeField] private Image UI_Image;
        [SerializeField] private TextMeshProUGUI label;

        public enum MenuMode { SelectLoad, SelectDelete, ConfirmDelete }
        public MenuMode currentMode = MenuMode.SelectLoad;

        public override void OnOpenMenu()
        {
            base.OnOpenMenu();
            GameManager.Instance.currentSaveSlot = 1;
            SaveSystem.sceneLoaded += OnSceneLoaded;
            SetMode(MenuMode.SelectLoad);
            label.SetText("Load Game");
        }

        void SetMode(MenuMode mode)
        {
            currentMode = mode;
            switch (mode)
            {
                case MenuMode.SelectLoad:
                    UI_Image.sprite = normalSprite;
                    deleteText.SetActive(true);
                    warningUI.SetActive(false);
                    label.SetText("Load Game");
                    break;
                case MenuMode.SelectDelete:
                    UI_Image.sprite = deleteSprite;
                    deleteText.SetActive(false);
                    warningUI.SetActive(false);
                    label.SetText("Delete Game");
                    break;
                case MenuMode.ConfirmDelete:
                    warningUI.SetActive(true);
                    label.SetText("Delete Game");
                    break;
            }
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
            int active_slot = GameManager.Instance.currentSaveSlot;
            switch (currentMode)
            {
                case MenuMode.SelectLoad:
                    LoadGame(active_slot);
                    break;
                case MenuMode.SelectDelete:
                    SetMode(MenuMode.ConfirmDelete);
                    break;
            }
        }

        private void LoadGame(int slot)
        {
            AudioManager.instance.StopMusic();
            InputManager.Instance.isInputActive = false;
            EventSystem.current.SetSelectedGameObject(null, null);
            if (SaveSystem.HasSavedGameInSlot(slot))
                SaveSystem.LoadFromSlot(slot);
            else
                SaveSystem.RestartGame(m_startingArea);
        }

        private void DeleteGame(int slot)
        {
            SaveSystem.DeleteSavedGameInSlot(slot);
            saveSlots[slot - 1].SetButtonContent(); // Update UI after deletion
        }

        public override void OnPlayerDeleteInput()
        {
            if (currentMode == MenuMode.SelectLoad)
                SetMode(MenuMode.SelectDelete);
            else if(currentMode == MenuMode.ConfirmDelete)
            {
                int active_slot = GameManager.Instance.currentSaveSlot;
                DeleteGame(active_slot);
                SetMode(MenuMode.SelectDelete);
            }    
        }

        public override void OnBackPressed()
        {
            if (currentMode == MenuMode.SelectLoad)
            {
                if (MenuManager.Instance != null)
                {
                    MenuManager.Instance.CloseMenu();
                }
            }
            else if(currentMode == MenuMode.SelectDelete)
                SetMode(MenuMode.SelectLoad);
            else if (currentMode == MenuMode.ConfirmDelete)
                SetMode(MenuMode.SelectDelete);
        }
    }
}