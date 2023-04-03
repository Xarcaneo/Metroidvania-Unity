using UnityEngine;
using System.Collections;
using FMODUnity;

public class PlayAnimationEvents : MonoBehaviour
{
    private void PlaySound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }
}