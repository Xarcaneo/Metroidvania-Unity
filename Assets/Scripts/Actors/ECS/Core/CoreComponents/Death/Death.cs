using UnityEngine;
using System;

/// <summary>
/// Base class for handling entity death behavior.
/// Provides core functionality for death detection and handling.
/// </summary>
public abstract class Death : CoreComponent
{
    #region Events

    /// <summary>
    /// Event triggered when the entity dies.
    /// </summary>
    public event Action OnDeath;

    #endregion

    #region Protected Fields

    /// <summary>
    /// Reference to the entity's Stats component.
    /// </summary>
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    /// <summary>
    /// Flag indicating if the entity is dead.
    /// </summary>
    protected bool isDead;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Sets up the death detection system.
    /// </summary>
    protected virtual void Start()
    {
        if (Stats != null)
        {
            Stats.HealthZero += HandleDeath;
        }
    }

    /// <summary>
    /// Cleans up event subscriptions.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (stats != null)
        {
            Stats.HealthZero -= HandleDeath;
        }
    }

    #endregion

    #region Death Handling

    /// <summary>
    /// Handles the death of the entity.
    /// </summary>
    protected virtual void HandleDeath()
    {
        if (!isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
            Die();
        }
    }

    /// <summary>
    /// Implements the specific death behavior.
    /// Override this in derived classes to define custom death effects.
    /// </summary>
    public virtual void Die() { }

    /// <summary>
    /// Resets the death state.
    /// </summary>
    public virtual void ResetDeath()
    {
        isDead = false;
    }

    #endregion
}
