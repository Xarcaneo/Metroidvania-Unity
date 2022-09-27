using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class AudioSettingsMenu : Menu<AudioSettingsMenu>
    {
        [SerializeField] AudioMixer audioMixer;

        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        public override void OnStart()
        {
            base.OnStart();

            LoadAudioSettings();
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", volume);
            audioMixer.SetFloat("SFX2Volume", volume);
        }

        public override void OnReturnInput()
        {
            if (menuInput.actions["Return"].triggered)
            {
                SaveAudioSettings();
                OnBackPressed();
            }
        }

        private void SaveAudioSettings()
        {
            float volume = masterVolumeSlider.value;
            PlayerPrefs.SetFloat("MasterVolume", volume);

            volume = musicVolumeSlider.value;
            PlayerPrefs.SetFloat("MusicVolume", volume);

            volume = sfxVolumeSlider.value;
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        private void LoadAudioSettings()
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume");
            masterVolumeSlider.value = volume;

            volume = PlayerPrefs.GetFloat("MusicVolume");
            musicVolumeSlider.value = volume;

            volume = PlayerPrefs.GetFloat("SFXVolume");
            sfxVolumeSlider.value = volume;
        }
    }
}