using UnityEngine;

/// <summary>
/// Manages spawn point properties and initial player state.
/// Used to define locations where the player can spawn and their initial facing direction.
/// </summary>
public class FallbackSpawnPoint : MonoBehaviour
{
    #region Public Fields
    /// <summary>
    /// Determines if the player should face left when spawning at this point
    /// </summary>
    [Tooltip("If true, player will face left when spawning at this point")]
    public bool shouldFaceLeft;
    #endregion
}
