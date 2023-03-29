using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class DeathMenu : Menu<DeathMenu>
    {
        public override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

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
            int active_slot = GameManager.Instance.currentSaveSlot;

            if (SaveSystem.HasSavedGameInSlot(active_slot))
            {
                var savedGameData = SaveSystem.storer.RetrieveSavedGameData(active_slot);
                var positionData = SaveSystem.Deserialize<PlayerPositionSaver.PositionData>(savedGameData.GetData("playerPositionKey"));
                savedGameData.sceneName = positionData.checkpointSceneName;

                SaveSystem.LoadGame(savedGameData);
            }
            else
            {
                SaveSystem.RestartGame("Area 0");
            }
        }
    }
}