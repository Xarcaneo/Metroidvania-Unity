using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class DeathMenu : Menu<DeathMenu>
    {
        private bool canReturn = false;

        public override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            int active_slot = GameManager.Instance.currentSaveSlot;

            SaveSystem.sceneLoaded += OnSceneLoaded;
            GetComponent<Animator>().Play("FadeIn");
        }

        void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded -= OnSceneLoaded;
            GameMenu.Open();
        }

        public override void OnReturnInput() 
        {
            if (canReturn)
            {
                int active_slot = GameManager.Instance.currentSaveSlot;

                if (SaveSystem.HasSavedGameInSlot(active_slot))
                {
                    SaveSystem.LoadFromSlot(active_slot);
                }
                else
                {
                    SaveSystem.RestartGame("Area 0");
                }
            }

            canReturn = false;
        }

        public void OnAnimationFinish()
        {
            canReturn = true;
        }
    }
}