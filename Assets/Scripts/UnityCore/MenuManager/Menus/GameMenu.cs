using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System;
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

            //DialogueManager.dialogueUI = dialogueUI;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GameEvents.Instance.onPlayerDied += OnPlayerDied;
        }



        protected override void OnDisable()
        {
            base.OnDisable();

            GameEvents.Instance.onPlayerDied -= OnPlayerDied;
        }

        public override void OnReturnInput() => OnPausePressed();
        public override void OnPlayerMenuInput() => PlayerMenu.Open();

        private void OnPlayerDied()
        {
            if (SaveSystem.HasSavedGameInSlot(GameManager.Instance.currentSaveSlot))
                SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);

            DeathMenu.Open();
        }
        

        public void OnPausePressed()
        {
            PauseMenu.Open();
        }        
        

    }
}