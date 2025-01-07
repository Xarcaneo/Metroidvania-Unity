using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PixelCrushers;

namespace Menu
{
    /// <summary>
    /// Handles the play menu, allowing the player to load or delete saved games.
    /// Manages different menu modes such as selecting a save slot, deleting a save, and confirming deletion.
    /// </summary>
    public class PlayMenu : Menu<PlayMenu>
    {
        /// <summary>
        /// The name of the starting area to load if no saved game is found.
        /// </summary>
        [SerializeField] private string m_startingArea = "Vestibule 0";

        /// <summary>
        /// The list of save slot buttons displayed in the menu.
        /// </summary>
        [SerializeField] private List<SaveSlot> saveSlots;

        /// <summary>
        /// The sprite used for normal menu mode.
        /// </summary>
        [SerializeField] private Sprite normalSprite;

        /// <summary>
        /// The sprite used for delete menu mode.
        /// </summary>
        [SerializeField] private Sprite deleteSprite;

        /// <summary>
        /// The UI element that displays delete mode text.
        /// </summary>
        [SerializeField] private GameObject deleteText;

        /// <summary>
        /// The UI element that displays warning messages.
        /// </summary>
        [SerializeField] private GameObject warningUI;

        /// <summary>
        /// The image component used to display the current menu mode sprite.
        /// </summary>
        [SerializeField] private Image UI_Image;

        /// <summary>
        /// The label used to display the current menu mode text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI label;

        /// <summary>
        /// Represents the current mode of the menu.
        /// </summary>
        public enum MenuMode { SelectLoad, SelectDelete, ConfirmDelete }
        public MenuMode currentMode = MenuMode.SelectLoad;

        /// <summary>
        /// Called when the play menu is opened.
        /// Initializes the menu state and registers the scene loaded event.
        /// </summary>
        public override void OnOpenMenu()
        {
            base.OnOpenMenu();
            GameManager.Instance.currentSaveSlot = 1;
            SetMode(MenuMode.SelectLoad);
        }

        /// <summary>
        /// Called when the component is enabled.
        /// Registers the scene loaded event handler.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Called when the component is disabled.
        /// Unregisters the scene loaded event handler.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            SaveSystem.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Sets the UI state based on the given parameters.
        /// </summary>
        private void UpdateUIState(Sprite sprite, bool deleteTextActive, bool warningUIActive, string labelText)
        {
            UI_Image.sprite = sprite;
            deleteText.SetActive(deleteTextActive);
            warningUI.SetActive(warningUIActive);
            label.SetText(labelText);
        }

        /// <summary>
        /// Sets the current menu mode and updates the UI accordingly.
        /// </summary>
        void SetMode(MenuMode mode)
        {
            currentMode = mode;
            switch (mode)
            {
                case MenuMode.SelectLoad:
                    UpdateUIState(normalSprite, true, false, "Load Game");
                    break;
                case MenuMode.SelectDelete:
                    UpdateUIState(deleteSprite, false, false, "Delete Game");
                    break;
                case MenuMode.ConfirmDelete:
                    UpdateUIState(deleteSprite, false, true, "Delete Game");
                    break;
            }
        }

        /// <summary>
        /// Handles the scene loaded event.
        /// Opens the game menu and initializes necessary components after the scene is loaded.
        /// </summary>
        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            GameMenu.Open();
            GameManager.Instance.UpdateBindingToDialogue();
            GameEvents.Instance.NewSession();
        }

        /// <summary>
        /// Handles the action when a save slot is pressed.
        /// </summary>
        public void OnSlotPressed()
        {
            int active_slot = GameManager.Instance != null ? GameManager.Instance.currentSaveSlot : 1;
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

        /// <summary>
        /// Loads the game from the specified save slot or starts a new game if no save is found.
        /// </summary>
        private void LoadGame(int slot)
        {
            if (AudioManager.instance != null) AudioManager.instance.StopMusic();
            if (InputManager.Instance != null) InputManager.Instance.isInputActive = false;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null, null);

            if (SaveSystem.HasSavedGameInSlot(slot))
            {
                SaveSystem.LoadFromSlot(slot);
            }
            else
            {
                SaveSystem.RestartGame(m_startingArea);
            }
        }

        /// <summary>
        /// Deletes the game from the specified save slot and updates the UI.
        /// </summary>
        private void DeleteGame(int slot)
        {
            SaveSystem.DeleteSavedGameInSlot(slot);
            saveSlots[slot - 1].SetButtonContent();
        }

        /// <summary>
        /// Handles the delete input to switch between delete modes or confirm deletion.
        /// </summary>
        public override void OnPlayerDeleteInput()
        {
            if (currentMode == MenuMode.SelectLoad)
                SetMode(MenuMode.SelectDelete);
            else if (currentMode == MenuMode.ConfirmDelete)
            {
                int active_slot = GameManager.Instance.currentSaveSlot;
                DeleteGame(active_slot);
                SetMode(MenuMode.SelectDelete);
            }
        }

        /// <summary>
        /// Handles the back button press to navigate between menu modes.
        /// </summary>
        public override void OnBackPressed()
        {
            if (currentMode == MenuMode.SelectLoad)
            {
                if (MenuManager.Instance != null)
                {
                    MenuManager.Instance.CloseMenu();
                }
            }
            else if (currentMode == MenuMode.SelectDelete)
                SetMode(MenuMode.SelectLoad);
            else if (currentMode == MenuMode.ConfirmDelete)
                SetMode(MenuMode.SelectDelete);
        }
    }
}
