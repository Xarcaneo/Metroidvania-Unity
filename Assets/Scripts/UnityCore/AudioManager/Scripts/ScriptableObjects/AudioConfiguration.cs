using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Scriptable Objects/Audio/AudioConfiguration")]
    public class AudioConfiguration : ScriptableObject
    {
        #region Variables
        [Header("Mixer Group")]
        public AudioMixerGroup OutputAudioMixerGroup = null;

        [Header("Sound Properties")]
        public bool Mute = false;
        public bool PlayOnAwake = false;
        public bool Loop = false;
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(-3f, 3f)] public float Pitch = 1f;
        [Range(-1f, 1f)] public float PanStereo = 0f;
        [Range(0f, 1.1f)] public float ReverbZoneMix = 1f;

        [Header("Spatialisation")]
        [Range(0f, 1f)] public float SpatialBlend = 0f;
        public AudioRolloffMode RollofMode = AudioRolloffMode.Logarithmic;
        [Range(0.1f, 5f)] public float MinDistance = 0.1f;
        [Range(5f, 100f)] public float MaxDistance = 50f;
        [Range(0, 360)] public float Spread = 0;
        [Range(0f, 5f)] public float DopplerLevel = 1f;

        [Header("Ignores")]
        public bool BypassEffects = false;
        public bool BypassListenerEffects = false;
        public bool BypassReverbZones = false;
        public bool IgnoreListenerVolume = false;
        public bool IgnoreListenerPause = false;

        [Header("Custom Curve")]
        public AnimationCurve Curve = new AnimationCurve();
        #endregion

        public void ApplySettings(AudioSource audioSource)
        {
            audioSource.outputAudioMixerGroup = OutputAudioMixerGroup;
            audioSource.mute = Mute;
            audioSource.playOnAwake = PlayOnAwake;

            audioSource.loop = Loop;
            audioSource.volume = Volume;
            audioSource.pitch = Pitch;

            audioSource.panStereo = PanStereo;
            audioSource.spatialBlend = SpatialBlend;
            audioSource.reverbZoneMix = ReverbZoneMix;

            audioSource.dopplerLevel = DopplerLevel;
            audioSource.spread = Spread;
            audioSource.rolloffMode = RollofMode;

            if (RollofMode == AudioRolloffMode.Custom)
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, Curve);

            audioSource.minDistance = MinDistance;
            audioSource.maxDistance = MaxDistance;

            audioSource.bypassEffects = BypassEffects;
            audioSource.bypassListenerEffects = BypassListenerEffects;
            audioSource.bypassReverbZones = BypassReverbZones;
            audioSource.ignoreListenerVolume = IgnoreListenerVolume;
            audioSource.ignoreListenerPause = IgnoreListenerPause;
        }
    }
}