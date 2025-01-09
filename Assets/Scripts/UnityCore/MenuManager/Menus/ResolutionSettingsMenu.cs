using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Handles the resolution settings menu, including displaying available resolutions and applying selected resolutions.
    /// </summary>
    public class ResolutionSettingsMenu : Menu<ResolutionSettingsMenu>
    {
        /// <summary>
        /// The prefab for resolution buttons.
        /// </summary>
        [SerializeField] private ResolutionButton prefabResolutionButton;

        /// <summary>
        /// The parent GameObject that holds the resolution buttons.
        /// </summary>
        [SerializeField] private GameObject buttonsParent;

        /// <summary>
        /// The header text displaying the current resolution.
        /// </summary>
        [SerializeField] private TextMeshProUGUI headerText;

        /// <summary>
        /// A list of available screen resolutions.
        /// </summary>
        private List<Resolution> resolutions = new List<Resolution>
        {
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 2560, height = 1440 },
            new Resolution { width = 3840, height = 2160 }
        };

        /// <summary>
        /// Called when the resolution settings menu is opened.
        /// </summary>
        public override void OnOpenMenu()
        {
            base.OnOpenMenu();

            // Find the button that matches current resolution
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;

            // Find all resolution buttons
            ResolutionButton[] buttons = buttonsParent.GetComponentsInChildren<ResolutionButton>();

            // Find and focus the button that matches current resolution
            foreach (ResolutionButton button in buttons)
            {
                Resolution buttonRes = resolutions[button.buttonIndex];
                if (buttonRes.width == currentWidth && buttonRes.height == currentHeight)
                {
                    buttonToFocus = button.gameObject;
                    break;
                }
            }
        }

        /// <summary>
        /// Called when the resolution settings menu starts.
        /// Initializes the resolution buttons and sets the header.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();

            for (int i = 0; i < resolutions.Count; i++)
            {
                ResolutionButton tempButton = Instantiate(prefabResolutionButton, buttonsParent.transform);
                var textMeshPro = tempButton.GetComponentInChildren<TextMeshProUGUI>();
                textMeshPro.text = $"{resolutions[i].width} x {resolutions[i].height}";
                tempButton.buttonIndex = i;

                if (i == 0) buttonToFocus = tempButton.gameObject;

                tempButton.Pressed += OnResolutionButtonPressed;
            }
        }
 
        /// <summary>
        /// Handles the custom update logic for the resolution settings menu.
        /// </summary>
        public override void CustomUpdate()
        {
            base.CustomUpdate();
        }

        /// <summary>
        /// Called when a resolution button is pressed.
        /// Sets the screen resolution based on the selected resolution.
        /// </summary>
        /// <param name="index">The index of the selected resolution in the list.</param>
        public void OnResolutionButtonPressed(int index)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
