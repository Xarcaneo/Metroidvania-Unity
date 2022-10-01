using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioManagerData", menuName = "Scriptable Objects/Audio/AudioManagerData")]
    public class AudioManagerData : ScriptableObject
    {
        #region Variables
        public float MasterVolume = 1f;
        public float MusicVolume = 0.75f;
        public float SFXVolume = 0.75f;
        #endregion

        #region Even Methods
        public event UnityAction<string, float> SetVolume;
        public event UnityAction SaveData;

        public void Init()
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        }

        public void RaiseSetVolumeEvent(string exposedParameter, float volume)
        {
            SetVolume?.Invoke(exposedParameter, volume);
        }

        public void RaiseSaveDataEvent()
        {
            SaveData?.Invoke();
        }
        #endregion
    }
}
