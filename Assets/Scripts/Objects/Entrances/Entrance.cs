using PixelCrushers;
using UnityEngine;

/// <summary>
/// Base class for scene entrances that handles portal interactions and input management.
/// </summary>
public class Entrance : MonoBehaviour
{
    [SerializeField] protected ScenePortal scenePortal;

    public virtual void Start()
    {
        if (scenePortal != null)
        {
            scenePortal.onUsePortal.AddListener(EntranceEntered);
        }
        else
        {
            Debug.LogWarning($"[Entrance] ScenePortal not assigned on {gameObject.name}");
        }
    }

    public virtual void OnDestroy()
    {
        if (scenePortal != null && scenePortal.onUsePortal != null)
        {
            scenePortal.onUsePortal.RemoveListener(EntranceEntered);
        }

        if (InputManager.Instance != null)
        {
            InputManager.Instance.isInputActive = true;
        }
    }

    /// <summary>
    /// Virtual method called when the entrance is entered through the portal.
    /// Override this in derived classes to implement specific entrance behavior.
    /// </summary>
    public virtual void EntranceEntered() { }
}
