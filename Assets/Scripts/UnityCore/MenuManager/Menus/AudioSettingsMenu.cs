using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    /// <summary>
    /// Handles the audio settings menu, allowing players to adjust master, music, and SFX volumes.
    /// </summary>
    public class AudioSettingsMenu : Menu<AudioSettingsMenu>
    {
        [Header("Mixer Channels")]
        [SerializeField] private string MasterVolume = "MasterVolume";
        [SerializeField] private string MusicVolume = "MusicVolume";
        [SerializeField] private string SfxVolume = "SFXVolume";

        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        /// <summary>
        /// Called when the audio settings menu starts.
        /// Initializes the volume sliders to reflect saved preferences.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();
        }

        /// <summary>
        /// Called when the audio settings menu is enabled.
        /// Updates the volume sliders to reflect the current settings.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            SetSliders();
        }

        /// <summary>
        /// Sets the master volume.
        /// </summary>
        /// <param name="volume">The volume level to set.</param>
        public void SetMasterVolume(float volume)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.masterVolume = volume;
            }
            else
            {
                Debug.LogError("AudioManager instance is not available.");
            }
        }

        /// <summary>
        /// Sets the music volume.
        /// </summary>
        /// <param name="volume">The volume level to set.</param>
        public void SetMusicVolume(float volume)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.musicVolume = volume;
            }
            else
            {
                Debug.LogError("AudioManager instance is not available.");
            }
        }

        /// <summary>
        /// Sets the SFX volume.
        /// </summary>
        /// <param name="volume">The volume level to set.</param>
        public void SetSFXVolume(float volume)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.SFXVolume = volume;
                AudioManager.instance.SFX2Volume = volume;
            }
            else
            {
                Debug.LogError("AudioManager instance is not available.");
            }
        }

        /// <summary>
        /// Saves the audio settings and closes the menu.
        /// </summary>
        public override void OnReturnInput()
        {
            SaveAudioSettings();
            OnBackPressed();
        }

        /// <summary>
        /// Saves the current audio settings to player preferences.
        /// </summary>
        private void SaveAudioSettings()
        {
            if (AudioManager.instance != null)
            {
                float volume = AudioManager.instance.masterVolume;
                PlayerPrefs.SetFloat(MasterVolume, volume);

                volume = AudioManager.instance.musicVolume;
                PlayerPrefs.SetFloat(MusicVolume, volume);

                volume = AudioManager.instance.SFXVolume;
                PlayerPrefs.SetFloat(SfxVolume, volume);
            }
        }

        /// <summary>
        /// Sets the values of the volume sliders based on saved player preferences.
        /// </summary>
        private void SetSliders()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolume, 1f);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolume, 1f);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat(SfxVolume, 1f);
            }
        }
    }
}
