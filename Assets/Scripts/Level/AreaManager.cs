using UnityEngine;

/// <summary>
/// Manages the active area in the game and provides global access to the current area.
/// </summary>
public class AreaManager : MonoBehaviour
{
    #region Singleton Instance
    /// <summary>
    /// The singleton instance of the AreaManager.
    /// </summary>
    public static AreaManager Instance { get; private set; }
    #endregion

    #region Public Properties
    /// <summary>
    /// The currently active area.
    /// </summary>
    public Transform ActiveArea { get; private set; }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the active area.
    /// </summary>
    /// <param name="newArea">The transform of the new active area.</param>
    public void SetActiveArea(Transform newArea)
    {
        if (newArea != ActiveArea)
        {
            ActiveArea = newArea;
        }
    }
    #endregion
}
