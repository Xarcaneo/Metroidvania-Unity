using UnityEngine;

/// <summary>
/// Controls the visibility of a single pictogram (UI indicator) in the game world.
/// Provides a clean interface for enabling/disabling pictogram GameObjects.
/// </summary>
/// <remarks>
/// Features:
/// - Simple enable/disable functionality
/// - Automatic initialization to hidden state
/// - Optimized state changes with equality checking
/// - Safe GameObject reference handling
/// 
/// Usage:
/// Attach this component to objects that need to show/hide pictograms
/// based on game events or player interaction.
/// </remarks>
public class PictogramEnabler : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Reference to the pictogram GameObject to control
    /// </summary>
    [SerializeField]
    [Tooltip("The pictogram GameObject to show/hide")]
    private GameObject pictogram = null;
    #endregion

    #region Properties
    /// <summary>
    /// Controls the visibility state of the pictogram
    /// </summary>
    /// <remarks>
    /// Provides a clean interface to show/hide the pictogram while
    /// ensuring we don't make unnecessary state changes
    /// </remarks>
    internal new bool enabled
    {
        get => pictogram != null && pictogram.activeSelf;
        set
        {
            if (pictogram == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Pictogram reference is null!");
                return;
            }

            if (enabled == value) return;
            pictogram.SetActive(value);
        }
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the pictogram to a hidden state
    /// </summary>
    private void Awake()
    {
        if (pictogram == null)
        {
            Debug.LogError($"[{gameObject.name}] Pictogram reference not set in inspector!");
            return;
        }
        enabled = false;
    }

    /// <summary>
    /// Validates component setup in editor
    /// </summary>
    private void OnValidate()
    {
        if (pictogram == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Please assign a pictogram GameObject!");
        }
    }
    #endregion
}
