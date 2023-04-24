using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class ControlsMenu : Menu<ControlsMenu>
    {
        public override void OnBackPressed()
        {
            base.OnBackPressed();

            GameManager.Instance.UpdateBindingToDialogue();
        }
    }
}