using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        public void OnAudioPressed()
        {
            AudioSettingsMenu.Open();
        }

        public void OnGraphicPressed()
        {
            GraphicSettingsMenu.Open();
        }

        public void OnGamePressed()
        {
            GameSettingsMenu.Open();
        }
    }
}