using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    /// <summary>
    /// Handles the graphic settings menu, including fullscreen toggle and resolution settings navigation.
    /// </summary>
    public class GraphicSettingsMenu : Menu<GraphicSettingsMenu>
    {
        /// <summary>
        /// The toggle button for enabling or disabling fullscreen mode.
        /// </summary>
        [SerializeField] private Toggle fullscreenButton;

        /// <summary>
        /// Called when the graphic settings menu starts.
        /// Initializes the fullscreen button state.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            if (fullscreenButton != null)
            {
                fullscreenButton.isOn = Screen.fullScreen;
            }
            else
            {
                Debug.LogError("Fullscreen button is not assigned in the Inspector.");
            }
        }

        /// <summary>
        /// Called when the graphic settings menu is enabled.
        /// Syncs the fullscreen button state with the current screen state.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (fullscreenButton != null)
            {
                fullscreenButton.isOn = Screen.fullScreen;
            }
        }

        /// <summary>
        /// Opens the resolution settings menu.
        /// </summary>
        public void OnResolutionPressed()
        {
            ResolutionSettingsMenu.Open();
        }

        /// <summary>
        /// Toggles fullscreen mode based on the given value.
        /// </summary>
        /// <param name="fullscreen">True to enable fullscreen, false to disable it.</param>
        public void OnFullscreenToggle(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
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
