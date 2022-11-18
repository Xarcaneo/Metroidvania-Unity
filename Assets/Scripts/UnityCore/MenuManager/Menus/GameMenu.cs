using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class GameMenu : Menu<GameMenu>
    {
        [SerializeField] private HealthBarController healthBarController;

        public override void OnStart() => healthBarController.Initialize();
        public override void OnReturnInput() => OnPausePressed();
        public override void OnPlayerMenuInput() => PlayerMenu.Open();

        public void OnPausePressed()
        {
            PauseMenu.Open();
        }
    }
}