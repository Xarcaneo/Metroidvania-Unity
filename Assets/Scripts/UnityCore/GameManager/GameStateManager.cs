using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace UnityCore.GameManager
{
    /// <summary>
    /// Manages the overall game state, including transitions between gameplay and minigames.
    /// </summary>
    public class GameStateManager
    {
        public enum GameMode { GAMEPLAY, MINIGAME }
        public GameMode CurrentMode { get; private set; } = GameMode.GAMEPLAY;

        private string currentMinigame;
        private int[] currentPuzzleIds;

        public event Action<string> OnMinigameSetup;
        public event Action OnMinigameCancelled;

        public void OpenMinigame(string sceneName, int[] puzzleIds)
        {
            currentMinigame = sceneName;
            currentPuzzleIds = puzzleIds;
            CurrentMode = GameMode.MINIGAME;
            
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += OnMinigameSceneLoaded;
        }

        public void CloseMinigame(bool wasCancelled = false)
        {
            if (currentMinigame != null)
            {
                SceneManager.UnloadSceneAsync(currentMinigame);
                CurrentMode = GameMode.GAMEPLAY;
                
                if (wasCancelled)
                {
                    OnMinigameCancelled?.Invoke();
                }
                
                currentMinigame = null;
                currentPuzzleIds = null;
            }
        }

        private void OnMinigameSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == currentMinigame)
            {
                OnMinigameSetup?.Invoke(currentMinigame);
                SceneManager.sceneLoaded -= OnMinigameSceneLoaded;
            }
        }

        public int[] GetCurrentPuzzleIds() => currentPuzzleIds;
    }
}
