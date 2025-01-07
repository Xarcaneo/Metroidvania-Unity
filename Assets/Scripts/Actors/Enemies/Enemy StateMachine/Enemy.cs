using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an enemy entity in the game, inheriting from the base Entity class.
/// </summary>
public class Enemy : Entity
{
    /// <summary>
    /// Contains data specific to the enemy entity, such as health, attack power, etc.
    /// </summary>
    [Tooltip("The entity data defining the characteristics of this enemy.")]
    public D_Entity entityData;

    /// <summary>
    /// Retrieves the death state for the enemy. Override this to define enemy-specific death behavior.
    /// </summary>
    /// <returns>
    /// A <see cref="State"/> representing the death state of the enemy, or null if no specific state is defined.
    /// </returns>
    public override State GetDeathState()
    {
        return null;
    }

    /// <summary>
    /// Retrieves the hurt state for the enemy. Override this to define enemy-specific hurt behavior.
    /// </summary>
    /// <returns>
    /// A <see cref="State"/> representing the hurt state of the enemy, or null if no specific state is defined.
    /// </returns>
    public override State GetHurtState()
    {
        return null;
    }
}
