using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Represents the settings menu in the game, allowing navigation to specific settings submenus such as audio, graphics, and game settings.
    /// Inherits from a generic Menu class for consistent menu behavior.
    /// </summary>
    public class SettingsMenu : Menu<SettingsMenu>
    {
        /// <summary>
        /// Opens the audio settings menu when the corresponding button is pressed.
        /// </summary>
        public void OnAudioPressed()
        {
            AudioSettingsMenu.Open();
        }

        /// <summary>
        /// Opens the graphics settings menu when the corresponding button is pressed.
        /// </summary>
        public void OnGraphicPressed()
        {
            GraphicSettingsMenu.Open();
        }

        /// <summary>
        /// Opens the game settings menu when the corresponding button is pressed.
        /// </summary>
        public void OnGamePressed()
        {
            GameSettingsMenu.Open();
        }
    }
}
