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
    public AreaDetector ActiveArea { get; private set; }
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
    public void SetActiveArea(AreaDetector newArea)
    {
        if (newArea != ActiveArea)
        {
            ActiveArea = newArea;
        }
    }

    /// <summary>
    /// Removes existing player essences from the specified area.
    /// </summary>
    /// <param name="area">The area to clear of player essences.</param>
    public void ClearExistingEssences(AreaDetector area)
    {
        foreach (Transform child in area.transform)
        {
            PlayerEssence existingEssence = child.GetComponent<PlayerEssence>();
            if (existingEssence != null)
            {
                Destroy(existingEssence.gameObject);
            }
        }
    }

    /// <summary>
    /// Finds and returns the AreaDetector object with the specified room ID.
    /// </summary>
    /// <param name="roomID">The ID of the room to find.</param>
    /// <returns>The AreaDetector object if found, otherwise null.</returns>
    public AreaDetector GetAreaByID(int roomID)
    {
        foreach (Transform child in transform)
        {
            AreaDetector areaDetector = child.GetComponent<AreaDetector>();
            if (areaDetector != null && areaDetector.roomNumber == roomID)
            {
                return areaDetector;
            }
        }

        Debug.LogWarning($"[AreaManager] AreaDetector with room ID {roomID} not found!");
        return null;
    }
    #endregion
}
