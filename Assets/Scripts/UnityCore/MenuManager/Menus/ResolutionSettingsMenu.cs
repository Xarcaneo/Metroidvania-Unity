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
            if(headerText) headerText.text = "Resolution: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        }

        public override void OnStart()
        {
            base.OnStart();

            resolutions = new Resolution[3];

            resolutions[0] = new Resolution() { width = 1280, height = 720 };
            resolutions[1] = new Resolution() { width = 1366, height = 768 };
            resolutions[2] = new Resolution() { width = 1920, height = 1080 };

            for (int i = 0; i < resolutions.Length; i++)
            {
                ResolutionButton tempButton = Instantiate(prefabResolutionButton, buttonsParent.transform);
                TextMeshProUGUI textMeshPro = tempButton.GetComponent<TextMeshProUGUI>();
                textMeshPro.text = resolutions[i].width.ToString() + " x " + resolutions[i].height.ToString();
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