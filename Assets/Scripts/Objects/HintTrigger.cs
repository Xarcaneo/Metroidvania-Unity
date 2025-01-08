using UnityEngine;

/// <summary>
/// Manages a trigger area that shows/hides a hint box when the player enters/exits.
/// Handles the display of contextual hints in the game world.
/// </summary>
public class HintTrigger : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Reference to the hint box that will display the hint text")]
    private HintBox m_hintBox;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the hint box on start
    /// </summary>
    private void Start()
    {
        if (!ValidateComponents()) return;
        
        m_hintBox.SetHintText();
        m_hintBox.HideHintBox();
    }

    /// <summary>
    /// Shows the hint box when an object enters the trigger
    /// </summary>
    /// <param name="collision">The collider that entered the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ValidateComponents()) return;

        // Only show hint for player
        if (collision.CompareTag("Player"))
        {
            m_hintBox.SetHintText();
            m_hintBox.ShowHintBox();
        }
    }

    /// <summary>
    /// Hides the hint box when an object exits the trigger
    /// </summary>
    /// <param name="collision">The collider that exited the trigger</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!ValidateComponents()) return;

        // Only hide hint for player
        if (collision.CompareTag("Player"))
        {
            m_hintBox.HideHintBox();
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_hintBox == null)
        {
            Debug.LogError($"[{gameObject.name}] Hint box reference is missing!");
            return false;
        }

        return true;
    }
    #endregion
}
