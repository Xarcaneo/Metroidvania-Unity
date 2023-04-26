using UnityEngine;
using System.Collections;
using FMODUnity;

public class PlayAnimationEvents : MonoBehaviour
{
    [SerializeField] public bool muteSounds = false;

    private void PlaySound(string path)
    {
        if(!muteSounds)
            FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
}