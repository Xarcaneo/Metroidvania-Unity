using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class Jukebox : MonoBehaviour
    {
        #region Variables
        public List<AudioCollection> MusicClips = default;
        [SerializeField] private bool randomTrack = false;
        [SerializeField] private int listIndex = 0;
        [SerializeField] private int audioCollection = 0;

        private float songLength = 0;
        private bool IsSongPlaying = false;
        #endregion

        #region Unity Base Methods
        private void OnDisable() => StopSong();

        private void Update() => UpdateJukeBox();
        #endregion

        #region User Methods
        public void SetAudioCollection(int audioCollection)
        {
            if (audioCollection != this.audioCollection || !IsSongPlaying)
            {
                if (IsSongPlaying)
                    StopSong();
                
                this.audioCollection = audioCollection;
                StartJukeBox();
            }
            else
                return;
        }

        public void StartJukeBox()
        {
            if (randomTrack)
                listIndex = RandomTrack();

            PlaySong();
        }

        private void PlaySong()
        {
            //Reset bool
            IsSongPlaying = false;

            //Raise The stop audio event
            MusicClips[audioCollection].AudioCollectionSFX[listIndex].AudioGroup.RaiseStopAudioEvent(MusicClips[audioCollection].AudioCollectionSFX[listIndex].AudioGroup.AudioSource);

            //Set the song length
            songLength = MusicClips[audioCollection].AudioCollectionSFX[listIndex].AudioClip.length + MusicClips[audioCollection].AudioCollectionSFX[listIndex].TrackOffset;

            //Raise the play audio event
            MusicClips[audioCollection].AudioCollectionSFX[listIndex].AudioGroup.RaisePrimaryAudioEvent(MusicClips[audioCollection].AudioCollectionSFX[listIndex].AudioGroup.AudioSource,
                MusicClips[audioCollection].AudioCollectionSFX[listIndex], MusicClips[audioCollection].AudioCollectionSFX[listIndex].AudioConfiguration);

            //Set the bool
            IsSongPlaying = true;

            //Increase the index
            listIndex++;
        }

        private void StopSong()
        {
            //Reset bool
            IsSongPlaying = false;

            //Raise the stop audio event
            MusicClips[audioCollection].AudioCollectionSFX[listIndex - 1].AudioGroup.RaiseStopAudioEvent(MusicClips[audioCollection].AudioCollectionSFX[listIndex - 1].AudioGroup.AudioSource);

            //Reset index
            listIndex = 0;
        }

        private int RandomTrack()
        {
            int i = Random.Range(0, MusicClips[audioCollection].AudioCollectionSFX.Count);
            return i;
        }

        private void UpdateJukeBox()
        {
            //Substract the current time from the current music track time
            songLength -= Time.unscaledDeltaTime;

            //Check if the current song time left is less than zero
            if (songLength < 0)
            {
                //Check if current song index is less than the total song count , If so play the next song in the list.
                if (listIndex < MusicClips.Count)
                {
                    PlaySong();
                }
                //If the current song index is greater than the total song count, reset and play the 1st song.
                else
                {
                    listIndex = 0;
                    PlaySong();
                }
            }
        }
        #endregion
    }
}