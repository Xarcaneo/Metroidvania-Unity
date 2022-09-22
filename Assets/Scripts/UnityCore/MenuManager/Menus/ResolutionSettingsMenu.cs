using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menu
{
    public class ResolutionSettingsMenu : Menu<ResolutionSettingsMenu>
    {
        [SerializeField] private ResolutionButton prefabResolutionButton;
        [SerializeField] private GameObject buttonsParent;
        [SerializeField] private TextMeshProUGUI headerText;

        Resolution[] resolutions;

        public override void OnOpenMenu()
        {
            base.OnOpenMenu();
        }

        private void SetHeader()
        {
            headerText.text = "Resolution: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        }

        public override void OnStart()
        {
            base.OnStart();

            resolutions = Screen.resolutions;

            for (int i = 0; i < resolutions.Length; i++)
            {
                ResolutionButton tempButton = Instantiate(prefabResolutionButton, buttonsParent.transform);
                tempButton.text = resolutions[i].width.ToString() + " x " + resolutions[i].height.ToString();
                tempButton.buttonIndex = i;

                if (i == 0) buttonToFocus = tempButton.gameObject;

                tempButton.Pressed += OnResolutionButtonPressed;
            }

            SetHeader();
        }

        public override void CustomUpdate()
        {
            base.CustomUpdate();

            SetHeader();
        }

        public void OnResolutionButtonPressed(int index)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}