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

    void OnDestroy()
    {
        scenePortal.onUsePortal.RemoveListener(EntranceEntered);
        InputManager.Instance.isInputActive = true;
    }

    void EntranceEntered()
    {
        InputManager.Instance.isInputActive = false;

        Player.Instance.gameObject.SetActive(false);
        
        if(transform.localScale.x == -1)
            GameManager.Instance.shouldFlipPlayer = true;
    }
}