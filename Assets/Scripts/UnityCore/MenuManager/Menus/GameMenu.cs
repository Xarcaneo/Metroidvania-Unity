using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    /// <summary>
    /// Handles the main game menu, managing game modes, UI elements, and event subscriptions.
    /// </summary>
    public class GameMenu : Menu<GameMenu>
    {
        /// <summary>
        /// Enum representing different game modes.
        /// </summary>
        public enum GameMode { GAMEPLAY, MINIGAME };

        /// <summary>
        /// The current game mode.
        /// </summary>
        public GameMode gameMode = GameMode.GAMEPLAY;

        /// <summary>
        /// The name of the currently opened minigame.
        /// </summary>
        private string minigameOpened;

        /// <summary>
        /// The health bar controller component.
        /// </summary>
        [SerializeField] private PlayerHealthBarController healthBarController;

        /// <summary>
        /// The game hotbar component.
        /// </summary>
        [SerializeField] public GameHotbar gameHotbar;

        /// <summary>
        /// The location name indicator component.
        /// </summary>
        [SerializeField] public LocationNameIndicator locationNameIndicator;

        /// <summary>
        /// The quest monitor UI element.
        /// </summary>
        [SerializeField] private GameObject m_QuestMonitor;

        /// <summary>
        /// Called when the game menu starts. Initializes the health bar controller.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            if (healthBarController != null)
            {
                healthBarController.Initialize();
            }
            else
            {
                Debug.LogError("HealthBarController is not assigned in the Inspector.");
            }
        }

        /// <summary>
        /// Sets the canvas for the game menu and refreshes the game hotbar.
        /// </summary>
        public override void SetCanvas()
        {
            base.SetCanvas();

            if (gameHotbar != null && gameHotbar.gameObject.activeSelf == true)
            {
                gameHotbar.gameObject.SetActive(false);
                gameHotbar.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Called when the game menu is enabled. Subscribes to necessary game events.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onPlayerDied += OnPlayerDied;
                GameEvents.Instance.onToggleUI += OnToggleUI;
                GameEvents.Instance.onNewSession += onNewSession;
                GameEvents.Instance.onPuzzleOpen += onPuzzleOpen;
                GameEvents.Instance.onPuzzleClose += onPuzzleClose;
            }

            SceneManager.sceneUnloaded += onSceneUnloaded;
            SaveSystem.saveDataApplied += OnSaveDataApplied;
        }

        /// <summary>
        /// Called when the game menu is disabled. Unsubscribes from game events.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onPlayerDied -= OnPlayerDied;
                GameEvents.Instance.onToggleUI -= OnToggleUI;
                GameEvents.Instance.onNewSession -= onNewSession;
                GameEvents.Instance.onPuzzleOpen -= onPuzzleOpen;
                GameEvents.Instance.onPuzzleClose -= onPuzzleClose;
            }

            SceneManager.sceneUnloaded -= onSceneUnloaded;
            SaveSystem.saveDataApplied -= OnSaveDataApplied;
        }

        /// <summary>
        /// Called when save data is applied. Activates the quest monitor UI.
        /// </summary>
        private void OnSaveDataApplied()
        {
            if (m_QuestMonitor != null)
            {
                m_QuestMonitor.SetActive(true);
            }
        }

        /// <summary>
        /// Called when a scene is unloaded. Deactivates the quest monitor UI.
        /// </summary>
        /// <param name="arg0">The unloaded scene.</param>
        private void onSceneUnloaded(Scene arg0)
        {
            if (m_QuestMonitor != null)
            {
                m_QuestMonitor.SetActive(false);
            }
        }

        /// <summary>
        /// Handles the return input to pause the game.
        /// </summary>
        public override void OnReturnInput() => OnPausePressed();

        /// <summary>
        /// Opens the player menu when the player menu input is triggered.
        /// </summary>
        public override void OnPlayerMenuInput() => PlayerMenu.Open();

        /// <summary>
        /// Handles the player death event. Saves the game and opens the death menu.
        /// </summary>
        private void OnPlayerDied()
        {
            if (GameManager.Instance != null && SaveSystem.HasSavedGameInSlot(GameManager.Instance.currentSaveSlot))
            {
                SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);
            }

            if (m_QuestMonitor != null)
            {
                m_QuestMonitor.SetActive(false);
            }

            DeathMenu.Open();
        }

        /// <summary>
        /// Handles the pause button press. Opens the pause menu or closes a minigame.
        /// </summary>
        public void OnPausePressed()
        {
            if (gameMode == GameMode.GAMEPLAY)
            {
                PauseMenu.Open();
            }
            else
            {
                onPuzzleClose(minigameOpened);
            }
        }

        /// <summary>
        /// Toggles the UI visibility based on the given state.
        /// </summary>
        /// <param name="state">True to show the UI, false to hide it.</param>
        private void OnToggleUI(bool state)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(state);
            }
        }

        /// <summary>
        /// Handles the new session event. Activates the game hotbar and deactivates the quest monitor.
        /// </summary>
        private void onNewSession()
        {
            if (gameHotbar != null)
            {
                gameHotbar.gameObject.SetActive(true);
            }

            if (m_QuestMonitor != null)
            {
                m_QuestMonitor.SetActive(false);
            }
        }

        /// <summary>
        /// Closes a minigame by unloading its scene.
        /// </summary>
        /// <param name="puzzleName">The name of the minigame scene to unload.</param>
        private void onPuzzleClose(string puzzleName)
        {
            if (SceneManager.GetSceneByName(puzzleName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(puzzleName);
                gameMode = GameMode.GAMEPLAY;
            }
        }

        /// <summary>
        /// Opens a minigame by loading its scene additively.
        /// </summary>
        /// <param name="puzzleName">The name of the minigame scene to load.</param>
        private void onPuzzleOpen(string puzzleName)
        {
            gameMode = GameMode.MINIGAME;
            minigameOpened = puzzleName;
            SceneManager.LoadScene(puzzleName, LoadSceneMode.Additive);
        }
    }
}
