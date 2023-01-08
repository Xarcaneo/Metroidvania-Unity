using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    [SerializeField] private ScenePortal scenePortal;

    void Start()
    {
        scenePortal.onUsePortal.AddListener(EntranceEntered);
    }

    void EntranceEntered() => InputManager.Instance.isInputActive = false;

    private void OnDestroy() => InputManager.Instance.isInputActive = true;

}
