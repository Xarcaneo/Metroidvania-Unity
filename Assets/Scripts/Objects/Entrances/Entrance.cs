using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    [SerializeField] protected ScenePortal scenePortal;

    public virtual void Start()
    {
        scenePortal.onUsePortal.AddListener(EntranceEntered);
    }

    public virtual void OnDestroy()
    {
        scenePortal.onUsePortal.RemoveListener(EntranceEntered);
        InputManager.Instance.isInputActive = true;
    }

    public virtual void EntranceEntered() {}
}
