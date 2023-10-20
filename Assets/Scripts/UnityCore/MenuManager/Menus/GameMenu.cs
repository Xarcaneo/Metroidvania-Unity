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
        [SerializeField] public GameHotbar gameHotbar;

        [SerializeField] public LocationNameIndicator locationNameIndicator;

        public override void OnStart()
        {
            base.OnStart();

            healthBarController.Initialize();
        }

        public override void SetCanvas()
        {
            base.SetCanvas();

            if (gameHotbar.gameObject.activeSelf == true)
            {
                gameHotbar.gameObject.SetActive(false);
                gameHotbar.gameObject.SetActive(true);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GameEvents.Instance.onPlayerDied += OnPlayerDied;
            GameEvents.Instance.onToggleUI += OnToggleUI;
            GameEvents.Instance.onNewSession += onNewSession;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GameEvents.Instance.onPlayerDied -= OnPlayerDied;
            GameEvents.Instance.onToggleUI -= OnToggleUI;
            GameEvents.Instance.onNewSession -= onNewSession;
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

        private void OnToggleUI(bool state)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(state);
            }
        }

        private void onNewSession() => gameHotbar.gameObject.SetActive(true);
    }
}