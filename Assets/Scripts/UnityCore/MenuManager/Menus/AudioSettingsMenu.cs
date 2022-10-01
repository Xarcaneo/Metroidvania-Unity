using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class AudioSettingsMenu : Menu<AudioSettingsMenu>
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioManagerData audioManagerData;

        [Header("Mixer Channels")]
        [SerializeField] private string MasterVolume = "MasterVolume";
        [SerializeField] private string MusicVolume = "MusicVolume";
        [SerializeField] private string SfxVolume = "SFXVolume";
        [SerializeField] private string Sfx2Volume = "SFX2Volume";

        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        public override void OnStart()
        {
            base.OnStart();
            SetSliders();
        }

        public void SetMasterVolume(float volume)
        {
            audioManagerData.RaiseSetVolumeEvent(MasterVolume, volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioManagerData.RaiseSetVolumeEvent(MusicVolume, volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioManagerData.RaiseSetVolumeEvent(SfxVolume, volume);
            audioManagerData.RaiseSetVolumeEvent(Sfx2Volume, volume);
        }

        public override void OnReturnInput()
        {
            if (menuInput.actions["Return"].triggered)
            {
                audioManagerData.RaiseSaveDataEvent();
                SaveAudioSettings();
                OnBackPressed();
            }
        }

        private void SaveAudioSettings()
        {
            float volume = masterVolumeSlider.value;
            PlayerPrefs.SetFloat(MasterVolume, volume);

            volume = musicVolumeSlider.value;
            PlayerPrefs.SetFloat(MusicVolume, volume);

            volume = sfxVolumeSlider.value;
            PlayerPrefs.SetFloat(SfxVolume, volume);
        }

        private void SetSliders()
        {
            masterVolumeSlider.value = audioManagerData.MasterVolume;
            musicVolumeSlider.value = audioManagerData.MusicVolume;
            sfxVolumeSlider.value = audioManagerData.SFXVolume;
        }
    }
}