using UnityEngine;

/// <summary>
/// Concrete implementation of BaseHazard that can be used directly in the game.
/// This class provides the basic hazard functionality for instantly killing entities.
/// Attach this component to any GameObject that should act as a hazard in the game.
/// </summary>
/// <remarks>
/// When an entity collides with this hazard:
/// - If the entity implements IDamageable, it will be instantly killed
/// - If the entity is a player, their essence spawning will be disabled
/// Make sure the GameObject has a Collider2D component with "Is Trigger" enabled.
/// </remarks>
public class Hazard : BaseHazard
{
    /// <summary>
    /// Called when a Collider2D enters this hazard's trigger collider.
    /// Handles the instant kill functionality by calling the base class implementation.
    /// </summary>
    /// <param name="collision">The Collider2D that entered the trigger.</param>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
