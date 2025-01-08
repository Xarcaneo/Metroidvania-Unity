using UnityEngine;

/// <summary>
/// Manages multiple pictograms (UI indicators) in the game world.
/// Handles switching between different pictograms and their visibility states.
/// </summary>
/// <remarks>
/// Features:
/// - Multiple pictogram management
/// - Default pictogram support
/// - Safe switching between pictograms
/// - Show/hide functionality
/// 
/// Usage:
/// Attach this component to objects that need to manage multiple
/// pictograms and switch between them based on game state.
/// </remarks>
public class PictogramHandler : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Array of pictogram enablers to manage
    /// </summary>
    [SerializeField]
    [Tooltip("List of pictogram enablers to manage")]
    private PictogramEnabler[] pictograms = null;

    /// <summary>
    /// Index of the default pictogram to show
    /// </summary>
    [SerializeField]
    [Min(0)]
    [Tooltip("Index of the pictogram to show by default (0-based)")]
    private int defaultPictogram = 0;
    #endregion

    #region Private Fields
    /// <summary>
    /// Index of the currently active pictogram
    /// </summary>
    private int currentPictogram;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the handler with the default pictogram
    /// </summary>
    private void Awake()
    {
        ValidatePictograms();
        currentPictogram = defaultPictogram;
    }

    /// <summary>
    /// Validates component setup in editor
    /// </summary>
    private void OnValidate()
    {
        if (pictograms == null || pictograms.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No pictograms assigned!");
            return;
        }

        // Ensure default pictogram index is valid
        defaultPictogram = Mathf.Min(defaultPictogram, pictograms.Length - 1);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Shows a specific pictogram and hides the current one
    /// </summary>
    /// <param name="pictogramIndex">Index of the pictogram to show</param>
    /// <remarks>
    /// Will log a warning if the index is out of range
    /// </remarks>
    public void ShowPictogram(int pictogramIndex)
    {
        if (!IsValidIndex(pictogramIndex))
        {
            Debug.LogWarning($"[{gameObject.name}] Invalid pictogram index: {pictogramIndex}");
            return;
        }

        // Hide current pictogram
        if (IsValidIndex(currentPictogram))
        {
            pictograms[currentPictogram].enabled = false;
        }

        // Show new pictogram
        currentPictogram = pictogramIndex;
        pictograms[currentPictogram].enabled = true;
    }

    /// <summary>
    /// Hides the currently active pictogram
    /// </summary>
    public void HidePictogram()
    {
        if (IsValidIndex(currentPictogram))
        {
            pictograms[currentPictogram].enabled = false;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Validates pictogram array setup
    /// </summary>
    private void ValidatePictograms()
    {
        if (pictograms == null || pictograms.Length == 0)
        {
            Debug.LogError($"[{gameObject.name}] Pictogram array is null or empty!");
            return;
        }

        for (int i = 0; i < pictograms.Length; i++)
        {
            if (pictograms[i] == null)
            {
                Debug.LogError($"[{gameObject.name}] Pictogram at index {i} is null!");
            }
        }
    }

    /// <summary>
    /// Checks if a pictogram index is valid
    /// </summary>
    /// <param name="index">Index to check</param>
    /// <returns>True if the index is valid</returns>
    private bool IsValidIndex(int index)
    {
        return pictograms != null && index >= 0 && index < pictograms.Length;
    }
    #endregion
}
