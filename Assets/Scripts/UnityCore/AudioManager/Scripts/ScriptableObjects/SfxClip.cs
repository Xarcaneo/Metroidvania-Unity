using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "SfxClip", menuName = "Scriptable Objects/Audio/SfxClip")]
    public class SfxClip : ScriptableObject
    {
        #region Variables
        [Header("Audio Options")]
        public GameAudioGroup AudioGroup = null;
        public AudioConfiguration AudioConfiguration = null;
        public AudioClip AudioClip = null;
        [Space]
        public float TrackOffset = 1f;
        public float FadeDuration = 3f;

        [Header("Music Ruble Options")]
        public bool UseRumble = false;
        public float Amplitude = 1f;
        public int Samples = 120;
        public float UpdateStep = 0.1f;
        #endregion
    }
}