using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class GraphicSettingsMenu : Menu<GraphicSettingsMenu>
    {
        [SerializeField] private Toggle fullscreenButton;

        public override void OnStart()
        {
            base.OnStart();

            fullscreenButton.isOn = Screen.fullScreen;
        }

        public void OnResolutionPressed()
        {
            ResolutionSettingsMenu.Open();
        }

        public void OnFullscreenToggle(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }

        public override void OnBackPressed()
        {
            // or add extra logic here

            base.OnBackPressed();

            // add extra logic here
        }
    }
}