using UnityEngine;

/// <summary>
/// Base class for all hazardous objects in the game that can instantly kill entities.
/// Handles basic collision detection and instant kill functionality.
/// </summary>
public abstract class BaseHazard : MonoBehaviour
{
    /// <summary>
    /// Called when something collides with the hazard.
    /// Instantly kills any IDamageable entity and prevents essence spawning for players.
    /// </summary>
    /// <param name="collision">The collider that triggered the collision.</param>
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object can be damaged
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.InstantKill();
        }

        // Special handling for player death (no essence spawning)
        if (collision.CompareTag("Player"))
        {
            Player.Instance.Core.GetCoreComponent<PlayerDeath>().canSpawnEssence = false;
        }
    }
}
