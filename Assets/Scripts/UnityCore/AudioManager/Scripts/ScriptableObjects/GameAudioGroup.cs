using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    [CreateAssetMenu(fileName = "GameAudioGroup", menuName = "Scriptable Objects/Audio/GameAudioGroup")]
    public class GameAudioGroup : ScriptableObject
    {
        #region Variables
        public event UnityAction<AudioSource, SfxClip, AudioConfiguration> PrimaryAudioEvent;
        public event UnityAction<AudioSource, SfxClip, AudioConfiguration> SecondaryAudioEvent;

        public event UnityAction<AudioSource, SfxClip, AudioConfiguration> FadeInAudioEvent;
        public event UnityAction<AudioSource, SfxClip, AudioConfiguration> FadeOutAudioEvent;

        public event UnityAction<AudioSource    > StopAudioEvent;

        [HideInInspector] public AudioSource AudioSource = null;
        #endregion

        #region Event Methods
        public void RaisePrimaryAudioEvent(AudioSource audioSource, SfxClip audioClip, AudioConfiguration audioConfiguration)
        {
            PrimaryAudioEvent?.Invoke(audioSource, audioClip, audioConfiguration);
        }

        public void RaiseSecondaryAudioEvent(AudioSource audioSource, SfxClip audioClip, AudioConfiguration audioConfiguration)
        {
            SecondaryAudioEvent?.Invoke(audioSource, audioClip, audioConfiguration);
        }

        public void RaiseFadeInAudioEvent(AudioSource audioSource, SfxClip audioClip, AudioConfiguration audioConfiguration)
        {
            FadeInAudioEvent?.Invoke(audioSource, audioClip, audioConfiguration);
        }

        public void RaiseFadeOutAudioEvent(AudioSource audioSource, SfxClip audioClip, AudioConfiguration audioConfiguration)
        {
            FadeOutAudioEvent?.Invoke(audioSource, audioClip, audioConfiguration);
        }

        public void RaiseStopAudioEvent(AudioSource audioSource)
        {
            StopAudioEvent?.Invoke(audioSource);
        }
        #endregion
    }
}