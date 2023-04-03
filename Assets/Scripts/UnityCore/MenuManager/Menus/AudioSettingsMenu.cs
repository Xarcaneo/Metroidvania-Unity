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

        [Header("Mixer Channels")]

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

        }

        public void SetMusicVolume(float volume)
        {

        }

        public void SetSFXVolume(float volume)
        {

        }

        public override void OnReturnInput()
        {
            SaveAudioSettings();
            OnBackPressed();
        }

        private void SaveAudioSettings()
        {

        }

        private void SetSliders()
        {

        }
    }
}