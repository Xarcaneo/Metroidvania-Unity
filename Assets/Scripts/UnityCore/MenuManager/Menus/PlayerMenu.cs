using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Handles the player menu, including tabs and stats panel, and manages pause state.
    /// Inherits from a generic Menu class for consistent menu behavior.
    /// </summary>
    public class PlayerMenu : Menu<PlayerMenu>
    {
        /// <summary>
        /// The panel displaying player stats, used to show player information in the menu.
        /// </summary>
        [SerializeField] private StatsPanel statsPanel;

        /// <summary>
        /// The group of tabs used to navigate through different sections of the player menu.
        /// </summary>
        [SerializeField] private TabGroup tabGroup;

        /// <summary>
        /// The world map tab within the player menu, used to display the game's world map.
        /// </summary>
        [SerializeField] private WorldMapTab m_worldMapTab;

        /// <summary>
        /// Called when the player menu starts.
        /// Initializes panels and tabs with null checks to prevent runtime errors.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            if (statsPanel != null) statsPanel.Initialize();
            else Debug.LogError("StatsPanel is not assigned in the Inspector.");

            if (m_worldMapTab != null) m_worldMapTab.Initialize();
            else Debug.LogError("WorldMapTab is not assigned in the Inspector.");
        }

        /// <summary>
        /// Continuously updates the player menu.
        /// </summary>
        public override void CustomUpdate()
        {
            base.CustomUpdate();
        }

        /// <summary>
        /// Called when the player menu is opened.
        /// Pauses the game and resets tab pages.
        /// </summary>
        public override void OnOpenMenu()
        {
            SetPauseState(true);
            tabGroup.ResetPages();
        }

        /// <summary>
        /// Handles the return input to resume the game.
        /// </summary>
        public override void OnReturnInput() => OnResumePressed();

        /// <summary>
        /// Handles the player menu input to resume the game.
        /// </summary>
        public override void OnPlayerMenuInput() => OnResumePressed();

        /// <summary>
        /// Resumes the game by unpausing and closing the menu.
        /// </summary>
        public void OnResumePressed()
        {
            SetPauseState(false);
            base.OnBackPressed();
        }

        /// <summary>
        /// Sets the game's pause state and triggers the corresponding game event.
        /// </summary>
        /// <param name="isPaused">True to pause the game, false to resume.</param>
        private void SetPauseState(bool isPaused)
        {
            Time.timeScale = isPaused ? 0 : 1;
            GameEvents.Instance.PauseTrigger(isPaused);
        }
    }
}
