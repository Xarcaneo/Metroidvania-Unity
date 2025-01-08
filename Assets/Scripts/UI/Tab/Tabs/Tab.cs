using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all tab content in the UI system.
/// Provides a common interface for tab activation and management.
/// </summary>
public class Tab : MonoBehaviour
{
    /// <summary>
    /// Called when this tab becomes active. Override this method to implement
    /// custom behavior when the tab is selected.
    /// </summary>
    public virtual void OnActive() { }
}
