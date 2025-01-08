using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Handles the game settings menu, allowing navigation to language and controls settings menus.
    /// </summary>
    public class GameSettingsMenu : Menu<GameSettingsMenu>
    {
        /// <summary>
        /// Opens the language settings menu if available.
        /// </summary>
        public void OnLanguagePressed()
        {
            if (LanguageSettingsMenu.Instance != null)
            {
                LanguageSettingsMenu.Open();
            }
            else
            {
                Debug.LogError("LanguageSettingsMenu is not available.");
            }
        }

        /// <summary>
        /// Opens the controls settings menu if available.
        /// </summary>
        public void OnControlsPressed()
        {
            if (ControlsMenu.Instance != null)
            {
                ControlsMenu.Open();
            }
            else
            {
                Debug.LogError("ControlsMenu is not available.");
            }
        }

        /// <summary>
        /// Called when the game settings menu is enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// Called when the game settings menu is disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// Handles the back button press.
        /// Adds custom logic before and after closing the menu.
        /// </summary>
        public override void OnBackPressed()
        {
            // Add custom logic before closing the menu
            base.OnBackPressed();
            // Add custom logic after closing the menu
        }
    }
}
