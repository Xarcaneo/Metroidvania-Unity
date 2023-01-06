using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioCollection", menuName = "Scriptable Objects/Audio/AudioCollection")]
    public class AudioCollection : ScriptableObject
    {
        public List<SfxClip> AudioCollectionSFX;
    }
}