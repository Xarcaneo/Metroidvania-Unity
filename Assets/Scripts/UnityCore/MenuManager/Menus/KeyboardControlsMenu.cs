using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Handles the keyboard controls settings menu, allowing players to adjust keyboard-related settings.
    /// </summary>
    public class KeyboardControlsMenu : Menu<KeyboardControlsMenu>
    {
        /// <summary>
        /// Handles the back button press.
        /// Ensures that game bindings are updated upon returning from the controls menu.
        /// </summary>
        public override void OnBackPressed()
        {
            base.OnBackPressed();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateBindingToDialogue();
            }
            else
            {
                Debug.LogError("GameManager instance is not available.");
            }
        }
    }
}
