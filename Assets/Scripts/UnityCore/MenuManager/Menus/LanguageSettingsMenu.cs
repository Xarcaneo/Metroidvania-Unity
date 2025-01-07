using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Handles the language settings menu, allowing the player to change the game's language.
    /// </summary>
    public class LanguageSettingsMenu : Menu<LanguageSettingsMenu>
    {
        /// <summary>
        /// Sets the game's language when a language button is pressed.
        /// </summary>
        /// <param name="language">The language to set, represented as a string.</param>
        public void OnLanguageButtonPressed(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                Debug.LogWarning("Invalid language parameter.");
                return;
            }

            if (DialogueManager.instance != null)
            {
                DialogueManager.SetLanguage(language);
            }
            else
            {
                Debug.LogError("DialogueManager is not initialized.");
            }
        }
    }
}
