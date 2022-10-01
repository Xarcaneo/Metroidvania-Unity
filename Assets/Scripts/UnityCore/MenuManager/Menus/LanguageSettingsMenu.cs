using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class LanguageSettingsMenu : Menu<LanguageSettingsMenu>
    {
        public void OnLanguageButtonPressed(string language)
        {
            DialogueManager.SetLanguage(language);
        }
    }
}