using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class GameMenu : Menu<GameMenu>
    {
        [SerializeField] private PlayerHealthBarController healthBarController;
        [SerializeField] private StandardDialogueUI dialogueUI;
        public override void OnStart()
        {
            base.OnStart();

            healthBarController.Initialize();
        }

        public override void SetCanvas()
        {
            base.SetCanvas();

            DialogueManager.dialogueUI = dialogueUI;
        }

        public override void OnReturnInput() => OnPausePressed();
        public override void OnPlayerMenuInput() => PlayerMenu.Open();

        public void OnPausePressed()
        {
            PauseMenu.Open();
        }
    }
}