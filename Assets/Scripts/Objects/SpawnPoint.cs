using UnityEngine;

/// <summary>
/// Manages spawn point properties and initial player state.
/// Used to define locations where the player can spawn and their initial facing direction.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    #region Public Fields
    /// <summary>
    /// Determines if the player should face left when spawning at this point
    /// </summary>
    [Tooltip("If true, player will face left when spawning at this point")]
    public bool shouldFaceLeft;
    #endregion

    #region Serialized Fields
    [SerializeField]
    [Tooltip("Size of the spawn point gizmo in the editor")]
    private float m_gizmoSize = 0.5f;

    [SerializeField]
    [Tooltip("Length of the direction arrow gizmo")]
    private float m_arrowLength = 1f;
    #endregion

    #region Unity Editor
    /// <summary>
    /// Draws visual indicators in the Unity Editor
    /// Shows spawn point location and facing direction
    /// </summary>
    private void OnDrawGizmos()
    {
        DrawSpawnPointGizmo();
        DrawDirectionArrow();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Draws the spawn point sphere gizmo
    /// </summary>
    private void DrawSpawnPointGizmo()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_gizmoSize);
    }

    /// <summary>
    /// Draws an arrow indicating the player's facing direction
    /// </summary>
    private void DrawDirectionArrow()
    {
        Vector3 direction = shouldFaceLeft ? Vector3.left : Vector3.right;
        Gizmos.DrawRay(transform.position, direction * m_arrowLength);
    }
    #endregion
}
