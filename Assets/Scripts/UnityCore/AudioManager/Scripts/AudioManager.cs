using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Variables
        [Header("Audio Settings")]
        [SerializeField] private AudioMixer audioMixer = default;
        [SerializeField] private AudioManagerData managerData = default;

        [Header("Mixer Channels")]
        [SerializeField] private string MasterVolume = "MasterVolume";
        [SerializeField] private string MusicVolume = "MusicVolume";
        [SerializeField] private string SfxVolume = "SFXVolume";
        [SerializeField] private string Sfx2Volume = "SFX2Volume";

        [Header("Music Channels")]
        [SerializeField] private GameAudioGroup musicSource = default;
        [SerializeField] private AudioSource musicAudioSource;

        [Header("SFX Channels")]
        [SerializeField] private GameAudioGroup primarySfxSource = default;
        [SerializeField] private AudioSource primarySfxAudioSource;
        [Space]
        [SerializeField] private GameAudioGroup secondarySfxSource;
        [SerializeField] private AudioSource secondarySfxAudioSource;

        [Header("Debug")]
        [SerializeField] private bool debugEnabled = false;
        [Space]
        [Range(0f, 1f), SerializeField] private float masterVolume = 0f;
        [Range(0f, 1f), SerializeField] private float musicVolume = 0f;
        [Range(0f, 1f), SerializeField] private float sfxVolume = 0f;
        [Space]
        [SerializeField] private bool updateSliders = false;
        [SerializeField] private bool saveData = false;

        [SerializeField] public Jukebox Jukebox;

        private static AudioManager _instance;
        public static AudioManager Instance { get => _instance; }
        #endregion

        #region Unity Base Methods

        // initialize references
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Start()
        {
            managerData.Init();
            SetAudioData();
        }

        private void OnEnable()
        {
            managerData.SetVolume += SetVolume;
            managerData.SaveData += SaveAudioData;

            musicSource.PrimaryAudioEvent += PlayMusic;
            musicSource.StopAudioEvent += StopMusic;

            musicSource.FadeInAudioEvent += FadeInMusic;
            musicSource.FadeOutAudioEvent += FadeOutMusic;

            primarySfxSource.PrimaryAudioEvent += PlaySFX;

            secondarySfxSource.PrimaryAudioEvent += PlaySFX;
            secondarySfxSource.SecondaryAudioEvent += PlayMusic;
            secondarySfxSource.StopAudioEvent += StopMusic;

            primarySfxSource.PauseAudioEvent += PauseAudio;
            primarySfxSource.UnPauseAudioEvent += UnPauseAudio;

            SetMainAudioSources();
        }

        private void OnDisable()
        {
            SetAudioData();

            managerData.SetVolume -= SetVolume;
            managerData.SaveData -= SaveAudioData;

            musicSource.PrimaryAudioEvent -= PlayMusic;
            musicSource.StopAudioEvent -= StopMusic;

            musicSource.FadeInAudioEvent -= FadeInMusic;
            musicSource.FadeOutAudioEvent -= FadeOutMusic;

            primarySfxSource.PrimaryAudioEvent -= PlaySFX;

            secondarySfxSource.PrimaryAudioEvent -= PlaySFX;
            secondarySfxSource.SecondaryAudioEvent -= PlayMusic;
            secondarySfxSource.StopAudioEvent -= StopMusic;

            primarySfxSource.PauseAudioEvent -= PauseAudio;
            primarySfxSource.UnPauseAudioEvent -= UnPauseAudio;
        }

        private void OnValidate()
        {
            if (debugEnabled)
            {
                if (Application.isPlaying)
                {
                    SetVolume(MasterVolume, masterVolume);
                    SetVolume(MusicVolume, musicVolume);
                    SetVolume(SfxVolume, sfxVolume);
                }

                if (updateSliders)
                {
                    masterVolume = managerData.MasterVolume;
                    musicVolume = managerData.MusicVolume;
                    sfxVolume = managerData.SFXVolume;
                }

                if (saveData)
                {
                    SaveAudioData();
                }
            }
        }
        #endregion

        #region User Methods
        public void PlaySFX(AudioSource audioSource, SfxClip sfxClip, AudioConfiguration audioConfiguration)
        {
            audioConfiguration.ApplySettings(audioSource);
            audioSource.PlayOneShot(sfxClip.AudioClip);
        }

        public void PlayMusic(AudioSource audioSource, SfxClip sfxClip, AudioConfiguration audioConfiguration)
        {
            audioConfiguration.ApplySettings(audioSource);
            audioSource.clip = sfxClip.AudioClip;
            audioSource.Play();
        }

        public void FadeInMusic(AudioSource audioSource, SfxClip sfxClip, AudioConfiguration audioConfiguration)
        {
            audioConfiguration.ApplySettings(audioSource);
            StartCoroutine(FadeInAudio(audioSource, MusicVolume, sfxClip.AudioClip, sfxClip.FadeDuration));
        }
        public void FadeOutMusic(AudioSource audioSource, SfxClip sfxClip, AudioConfiguration audioConfiguration)
        {
            StartCoroutine(FadeOutAudio(audioSource, MusicVolume, sfxClip.FadeDuration));
        }

        public void StopMusic(AudioSource audioSource)
        {
            audioSource.Stop();
        }

        public void PauseAudio(AudioSource audioSource)
        {
            audioSource.Pause();
        }

        public void UnPauseAudio(AudioSource audioSource)
        {
            audioSource.UnPause();
        }

        public void SetVolume(string exposedParameter, float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1);
            audioMixer.SetFloat(exposedParameter, Mathf.Log10(volume) * 20);
        }

        public float GetVolume(string exposedParameter)
        {
            audioMixer.GetFloat(exposedParameter, out float volume);


            return volume = Mathf.Pow(10, volume / 20);
        }

        public void SetMainAudioSources()
        {
            musicSource.AudioSource = musicAudioSource;
            primarySfxSource.AudioSource = primarySfxAudioSource;
            secondarySfxSource.AudioSource = secondarySfxAudioSource;
        }

        public void SetAudioData()
        {
            SetVolume(MasterVolume, managerData.MasterVolume);
            SetVolume(MusicVolume, managerData.MusicVolume);
            SetVolume(SfxVolume, managerData.SFXVolume);
            SetVolume(Sfx2Volume, managerData.SFXVolume);
        }

        public void SaveAudioData()
        {
            managerData.MasterVolume = GetVolume(MasterVolume);
            managerData.MusicVolume = GetVolume(MusicVolume);
            managerData.SFXVolume = GetVolume(SfxVolume);
        }
        #endregion

        #region Corutines
        public IEnumerator FadeInAudio(AudioSource audioSource, string exposedParameters, AudioClip music, float duration)
        {
            float currentTime = 0;

            float currentVolume = GetVolume(exposedParameters);
            SetVolume(exposedParameters, 0);

            audioSource.clip = music;

            audioSource.Play();

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(0, currentVolume, currentTime / duration);
                SetVolume(exposedParameters, newVol);
                yield return null;
            }

            yield break;
        }

        public IEnumerator FadeOutAudio(AudioSource audioSource, string exposedParameters, float duration)
        {
            float currentTime = 0;

            float currentVolume = GetVolume(exposedParameters);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVolume, 0, currentTime / duration);
                SetVolume(exposedParameters, newVol);
                yield return null;
            }

            audioSource.Stop();

            SetVolume(exposedParameters, currentVolume);
            yield break;
        }

        #endregion
    }
}