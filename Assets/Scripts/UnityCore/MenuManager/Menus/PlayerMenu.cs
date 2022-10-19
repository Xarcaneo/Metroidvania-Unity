using UnityEngine;
using Audio;

namespace Menu
{
    public class PlayerMenu : Menu<PlayerMenu>
    {
        [SerializeField] private SfxClip sfxClip;

        public override void CustomUpdate()
        {
            base.CustomUpdate();
            OnPlayerMenuInput();
        }

        public override void OnOpenMenu()
        {
            Time.timeScale = 0;
            sfxClip.AudioGroup.RaisePauseAudioEvent(sfxClip.AudioGroup.AudioSource);
            GameEvents.Instance.PauseTrigger(true);
        }

        public override void OnReturnInput()
        {
            if (menuInput.actions["Return"].triggered)
            {
                OnResumePressed();
            }
        }

        private void OnPlayerMenuInput()
        {
            if (menuInput.actions["PlayerMenu"].triggered)
            {
                OnResumePressed();
            }
        }

        public void OnResumePressed()
        {
            Time.timeScale = 1;
            sfxClip.AudioGroup.RaiseUnPauseAudioEvent(sfxClip.AudioGroup.AudioSource);
            GameEvents.Instance.PauseTrigger(false);
            base.OnBackPressed();
        }
    }
}