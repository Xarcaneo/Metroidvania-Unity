using UnityEngine;

/// <summary>
/// Manages spawn point properties and initial player state.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Determines if the player should face left when spawning at this point.
    /// </summary>
    public bool shouldFaceLeft = false;

    private void OnDrawGizmos()
    {
        // Visual indicator in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw arrow to show facing direction
        Vector3 direction = shouldFaceLeft ? Vector3.left : Vector3.right;
        Gizmos.DrawRay(transform.position, direction);
    }
}
