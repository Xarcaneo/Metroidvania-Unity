using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int ChapterNumber = 0;

    private void OnEnable()
    {
        AudioManager.Instance.Jukebox.SetAudioCollection(ChapterNumber);
        AudioManager.Instance.Jukebox.gameObject.SetActive(true);
    }
}