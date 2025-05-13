using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityCore.GameManager;

namespace Menu
{
    /// <summary>
    /// Handles the main game menu, managing game modes, UI elements, and event subscriptions.
    /// </summary>
    public class GameMenu : Menu<GameMenu>
    {
        private static GameStateManager _gameStateManager;
        public static GameStateManager GameState => _gameStateManager ??= new GameStateManager();

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
                GameEvents.Instance.onNewSession += OnNewSession;
                GameEvents.Instance.onPuzzleOpen += OnPuzzleOpen;
                GameEvents.Instance.onPuzzleClose += OnPuzzleClose;
            }

            SceneManager.sceneUnloaded += OnSceneUnloaded;
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
                GameEvents.Instance.onNewSession -= OnNewSession;
                GameEvents.Instance.onPuzzleOpen -= OnPuzzleOpen;
                GameEvents.Instance.onPuzzleClose -= OnPuzzleClose;
            }

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
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
        private void OnSceneUnloaded(Scene arg0)
        {
            if (m_QuestMonitor != null)
            {
                m_QuestMonitor.SetActive(false);
            }
        }

        /// <summary>
        /// Handles the return input to pause the game.
        /// Only responds to keyboard input.
        /// </summary>
        public override void OnReturnInput()
        {
            // Only handle return input for keyboard
            if (InputDeviceDetector.CurrentInputDevice == InputDeviceDetector.InputDeviceType.Keyboard)
            {
                OnPausePressed();
            }
        }
        
        /// <summary>
        /// Handles the pause input to pause the game.
        /// Only responds to gamepad input.
        /// </summary>
        public override void OnPauseInput()
        {
            // Only handle pause input for gamepad
            if (InputDeviceDetector.CurrentInputDevice == InputDeviceDetector.InputDeviceType.Gamepad)
            {
                OnPausePressed();
            }
        }

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
            if (GameState.CurrentMode == GameStateManager.GameMode.GAMEPLAY)
            {
                PauseMenu.Open();
            }
            else
            {
                GameState.CloseMinigame(true);
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
        private void OnNewSession()
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
        private void OnPuzzleClose(string puzzleName)
        {
            GameState.CloseMinigame();
        }

        /// <summary>
        /// Opens a minigame by loading its scene additively.
        /// </summary>
        private void OnPuzzleOpen(string puzzleName, int[] puzzleIds)
        {
            GameState.OpenMinigame(puzzleName, puzzleIds ?? new int[] { 0 });
        }
    }
}
