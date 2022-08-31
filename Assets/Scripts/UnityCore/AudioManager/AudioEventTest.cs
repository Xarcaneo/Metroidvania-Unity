using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioEventTest : MonoBehaviour
    {
        #region Variables
        [Header("Audio Clips")]
        [SerializeField] SfxClip sfxClip = default;
        //[SerializeField] SfxClip heartbeatClip = default;
        [Space]
        [SerializeField] SfxClip musicClip1 = default;
        [SerializeField] SfxClip musicClip2 = default;

        [Header("Play Sfx")]
        [SerializeField] bool playSfx = false;
        [SerializeField] bool playHeartbeat = false;

        [Header("Play Music")]
        [SerializeField] bool playMusic = false;
        [SerializeField] bool stopMusic = false;

        [Header("Fade Music")]
        [SerializeField] bool fadeInMusic = false;
        [SerializeField] bool fadeOutMusic = false;
        [SerializeField] bool fadeClip1Clip2 = false;

        // Bool to stop event calls repeating as we are calling them via update.
        private bool eventEnabled = false;
        #endregion

        #region Unity Base Methods
        private void Update()
        {
            PlayAudio();
            FadeAudio();
        }
        #endregion

        #region User Methods
        private void PlayAudio()
        {
            if (playSfx && !eventEnabled)
            {
                // Set the event bool
                eventEnabled = true;

                // Raise the sfx clip primary audio event
                sfxClip.AudioGroup.RaisePrimaryAudioEvent(sfxClip.AudioGroup.AudioSource, sfxClip, sfxClip.AudioConfiguration);

                // Reset the event bool
                Invoke("ResetBool", 3f);

                Debug.Log("Play Sfx");
            }

            if (playHeartbeat && !eventEnabled)
            {
                // Set the event bool
                eventEnabled = true;

                // Raise the heartbeat clip secondary audio event
                //heartbeatClip.AudioGroup.RaiseSecondaryAudioEvent(heartbeatClip.AudioGroup.AudioSource, heartbeatClip, heartbeatClip.AudioConfiguration);

                // Set the rumble clip data
                //AudioRumble.UpdateClipData(heartbeatClip);

                Debug.Log("Play HeartBeat");
            }

            if (playMusic && !eventEnabled)
            {
                // Set the event bool
                eventEnabled = true;

                // Raise the music clip primary audio event
                musicClip1.AudioGroup.RaisePrimaryAudioEvent(musicClip1.AudioGroup.AudioSource, musicClip1, musicClip1.AudioConfiguration);

                Debug.Log("Play Music");
            }

            if (stopMusic && eventEnabled)
            {
                // Set the event bool
                eventEnabled = false;

                // Raise the music clip 1 stop audio event
                musicClip1.AudioGroup.RaiseStopAudioEvent(musicClip1.AudioGroup.AudioSource);

                // Raise the music clip 2 stop audio event
                musicClip2.AudioGroup.RaiseStopAudioEvent(musicClip2.AudioGroup.AudioSource);

                // Raise the heartbeat clip stop audio event
                //heartbeatClip.AudioGroup.RaiseStopAudioEvent(heartbeatClip.AudioGroup.AudioSource);

                // Reset the rumble clip data
                //AudioRumble.ResetClipData();

                Debug.Log("Stop Music");
            }
        }

        private void FadeAudio()
        {
            if (fadeInMusic && !eventEnabled)
            {
                // Set the event bool
                eventEnabled = true;

                // Raise the music clip 1 fade in audio event
                musicClip1.AudioGroup.RaiseFadeInAudioEvent(musicClip1.AudioGroup.AudioSource, musicClip1, musicClip1.AudioConfiguration);

                Debug.Log("Fade In Music");
            }

            if (fadeOutMusic && eventEnabled)
            {
                // Reset the event bool
                eventEnabled = false;

                // Raise the music clip 1 fade out audio event
                musicClip1.AudioGroup.RaiseFadeOutAudioEvent(musicClip1.AudioGroup.AudioSource, musicClip1, musicClip1.AudioConfiguration);

                Debug.Log("Fade Out Music");
            }

            if (fadeClip1Clip2 && eventEnabled)
            {
                // Reset the event bool
                eventEnabled = false;

                // Start the fade tracks method
                FadeTracks();

                Debug.Log("Starting Fade Between Tracks");
            }
        }

        private void ResetBool()
        {
            // Reset the event bool
            eventEnabled = false;

            Debug.Log("Reseting Bool");
        }

        private void FadeTracks()
        {
            // Start the coroutine
            StartCoroutine(FadeBetweenAudio(musicClip1, musicClip2));
        }
        #endregion

        #region Coroutines
        private IEnumerator FadeBetweenAudio(SfxClip clip1, SfxClip clip2)
        {
            // Raise the music clip 1 fade out audio event
            clip1.AudioGroup.RaiseFadeOutAudioEvent(clip1.AudioGroup.AudioSource, clip1, clip1.AudioConfiguration);

            yield return new WaitForSecondsRealtime(clip1.FadeDuration);

            // Raise the music clip 2 fade in audio event
            clip2.AudioGroup.RaiseFadeInAudioEvent(clip2.AudioGroup.AudioSource, clip2, clip2.AudioConfiguration);

            // Set the event bool
            eventEnabled = true;
        }
        #endregion
    }
}