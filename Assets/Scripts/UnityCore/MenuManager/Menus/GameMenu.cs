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
    public class GameMenu : Menu<GameMenu>
    {
        public enum GameMode { GAMEPLAY, MINIGAME };
        public GameMode gameMode = GameMode.GAMEPLAY;
        private string minigameOpened;

        [SerializeField] private PlayerHealthBarController healthBarController;
        [SerializeField] public GameHotbar gameHotbar;
        [SerializeField] public LocationNameIndicator locationNameIndicator;
        [SerializeField] private GameObject m_QuestMonitor;

        public override void OnStart()
        {
            base.OnStart();

            healthBarController.Initialize();
        }

        public override void SetCanvas()
        {
            base.SetCanvas();

            if (gameHotbar.gameObject.activeSelf == true)
            {
                gameHotbar.gameObject.SetActive(false);
                gameHotbar.gameObject.SetActive(true);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GameEvents.Instance.onPlayerDied += OnPlayerDied;
            GameEvents.Instance.onToggleUI += OnToggleUI;
            GameEvents.Instance.onNewSession += onNewSession;
            GameEvents.Instance.onPuzzleOpen += onPuzzleOpen;
            GameEvents.Instance.onPuzzleClose += onPuzzleClose;
            SceneManager.sceneUnloaded += onSceneUnloaded;
            SaveSystem.saveDataApplied += OnSaveDataApplied;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_QuestMonitor.SetActive(false);

            GameEvents.Instance.onPlayerDied -= OnPlayerDied;
            GameEvents.Instance.onToggleUI -= OnToggleUI;
            GameEvents.Instance.onNewSession -= onNewSession;
            GameEvents.Instance.onPuzzleOpen -= onPuzzleOpen;
            GameEvents.Instance.onPuzzleClose -= onPuzzleClose;
            SceneManager.sceneUnloaded -= onSceneUnloaded;
            SaveSystem.saveDataApplied -= OnSaveDataApplied;
        }

        private void OnSaveDataApplied() => m_QuestMonitor.SetActive(true);
        private void onSceneUnloaded(Scene arg0) => m_QuestMonitor.SetActive(false);

        public override void OnReturnInput() => OnPausePressed();
        public override void OnPlayerMenuInput() => PlayerMenu.Open();

        private void OnPlayerDied()
        {
            if (SaveSystem.HasSavedGameInSlot(GameManager.Instance.currentSaveSlot))
                SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);

            DeathMenu.Open();
        }
        public void OnPausePressed()
        {
            if (gameMode == GameMode.GAMEPLAY)
                PauseMenu.Open();
            else
                onPuzzleClose(minigameOpened);
        }

        private void OnToggleUI(bool state)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(state);
            }
        }

        private void onNewSession() => gameHotbar.gameObject.SetActive(true);
        private void onPuzzleClose(string puzzleName)
        {
            if (SceneManager.GetSceneByName(puzzleName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(puzzleName);
                gameMode = GameMode.GAMEPLAY;
            }
        }
        private void onPuzzleOpen(string puzzleName)
        {
            gameMode = GameMode.MINIGAME;
            minigameOpened = puzzleName;
            SceneManager.LoadScene(puzzleName, LoadSceneMode.Additive);
        }
    }
}