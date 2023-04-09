using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class AudioSettingsMenu : Menu<AudioSettingsMenu>
    {
        [Header("Mixer Channels")]
        [SerializeField] private string MasterVolume = "MasterVolume";
        [SerializeField] private string MusicVolume = "MusicVolume";
        [SerializeField] private string SfxVolume = "SFXVolume";

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
            AudioManager.instance.masterVolume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            AudioManager.instance.musicVolume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            AudioManager.instance.SFXVolume = volume;
            AudioManager.instance.SFX2Volume = volume;
        }

        public override void OnReturnInput()
        {
            SaveAudioSettings();
            OnBackPressed();
        }

        private void SaveAudioSettings()
        {
            float volume = AudioManager.instance.masterVolume;
            PlayerPrefs.SetFloat(MasterVolume, volume);

            volume = AudioManager.instance.musicVolume;
            PlayerPrefs.SetFloat(MusicVolume, volume);

            volume = AudioManager.instance.SFXVolume;
            PlayerPrefs.SetFloat(SfxVolume, volume);
        }

        private void SetSliders()
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolume, 1f);
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolume, 1f);
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(SfxVolume, 1f);
        }
    }
}