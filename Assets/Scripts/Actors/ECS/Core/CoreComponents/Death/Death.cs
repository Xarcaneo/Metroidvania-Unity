using UnityEngine;
using System;

/// <summary>
/// Base class for handling entity death behavior.
/// Provides core functionality for death detection and handling.
/// </summary>
public abstract class Death : CoreComponent
{
    /// <summary>
    /// Implements the specific death behavior.
    /// Override this in derived classes to define custom death effects.
    /// </summary>
    public virtual void Die() { }
}
