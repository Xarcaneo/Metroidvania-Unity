using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Menu
{
    public class MainMenu : Menu<MainMenu>
    {
        [SerializeField] SfxClip menuClip = default;

        [SerializeField] private float _playDelay = 0.5f;

        [SerializeField] private TransitionFader startTransitionPrefab;

        public override void OnOpenMenu()
        {
            menuClip.AudioGroup.RaiseFadeInAudioEvent(menuClip.AudioGroup.AudioSource, menuClip, menuClip.AudioConfiguration);
        }

        public void OnPlayPressed()
        {
            menuClip.AudioGroup.RaiseStopAudioEvent(menuClip.AudioGroup.AudioSource);
            StartCoroutine(OnPlayPressedRoutine());
        }

        private IEnumerator OnPlayPressedRoutine()
        {
            TransitionFader.PlayTransition(startTransitionPrefab);
            yield return new WaitForSeconds(_playDelay);
            Game.LevelLoader.Instance.LoadLevelAsync(1);
            GameMenu.Open();
        }

        public void OnSettingPressed()
        {
            SettingsMenu.Open();
        }

        public void OnCreditsPressed()
        {
            CreditsScreen.Open();
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}