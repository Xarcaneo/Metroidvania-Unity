using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Represents the death menu, allowing the player to return to a saved game or restart from a starting location.
    /// </summary>
    public class DeathMenu : Menu<DeathMenu>
    {
        /// <summary>
        /// The starting location to load if no saved game is found.
        /// </summary>
        [SerializeField] private string startingLocation = "Vestibule 0";

        /// <summary>
        /// Indicates whether the player can return to the game.
        /// </summary>
        private bool canReturn = false;

        /// <summary>
        /// Called when the death menu starts.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();
        }

        /// <summary>
        /// Called when the death menu is enabled.
        /// Subscribes to scene load events and plays the fade-in animation.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            int active_slot = GameManager.Instance != null ? GameManager.Instance.currentSaveSlot : 1;

            SaveSystem.sceneLoaded += OnSceneLoaded;

            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FadeIn");
            }
            else
            {
                Debug.LogError("Animator component is missing on DeathMenu.");
            }
        }

        /// <summary>
        /// Called when the death menu is disabled.
        /// Unsubscribes from scene load events.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            SaveSystem.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Handles the scene loaded event.
        /// Opens the game menu after the scene is loaded.
        /// </summary>
        /// <param name="sceneName">The name of the loaded scene.</param>
        /// <param name="sceneIndex">The index of the loaded scene.</param>
        private void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            GameMenu.Open();
        }

        /// <summary>
        /// Handles the return input to load a saved game or restart from the starting location.
        /// </summary>
        public override void OnReturnInput()
        {
            if (!canReturn) return;

            int active_slot = GameManager.Instance != null ? GameManager.Instance.currentSaveSlot : 1;

            if (SaveSystem.HasSavedGameInSlot(active_slot))
            {
                SaveSystem.LoadFromSlot(active_slot);
            }
            else
            {
                SaveSystem.RestartGame(startingLocation);
            }

            canReturn = false;
        }

        /// <summary>
        /// Sets the canReturn flag to true when the fade-in animation finishes.
        /// </summary>
        public void OnAnimationFinish()
        {
            canReturn = true;
        }
    }
}
