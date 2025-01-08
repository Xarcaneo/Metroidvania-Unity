using System;
using UnityEngine;

/// <summary>
/// Base class for all interactable objects in the game.
/// Provides core functionality for interaction state management and event handling.
/// </summary>
/// <remarks>
/// This class serves as the foundation for creating interactive objects in the game world.
/// It provides:
/// - Basic interaction state management
/// - Event system for interaction completion
/// - Virtual methods for custom interaction behavior
/// 
/// To create a new interactable object:
/// 1. Create a new class that inherits from Interactable
/// 2. Override the Interact() method to define custom behavior
/// 3. Call CallInteractionCompletedEvent() when the interaction is finished
/// </remarks>
public class Interactable : MonoBehaviour
{
    #region Events
    /// <summary>
    /// Event triggered when an interaction is completed.
    /// Subscribe to this event to respond to completed interactions.
    /// </summary>
    public event Action onInteractionCompleted;
    #endregion

    #region Public Properties
    /// <summary>
    /// Controls whether this object can be interacted with.
    /// Set to false to temporarily disable interactions.
    /// </summary>
    [SerializeField]
    [Tooltip("Whether this object can be interacted with")]
    public bool canInteract = true;

    /// <summary>
    /// Gets whether this object is currently being interacted with
    /// </summary>
    public bool IsInteracting => isInteracting;
    #endregion

    #region Protected Fields
    /// <summary>
    /// Indicates whether this object is currently being interacted with.
    /// Used to prevent multiple simultaneous interactions.
    /// </summary>
    protected bool isInteracting;
    #endregion

    #region Unity Messages
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Validates the initial setup of the interactable.
    /// </summary>
    protected virtual void OnValidate()
    {
        // Base validation can be extended in derived classes
    }

    /// <summary>
    /// Called when the object is destroyed.
    /// Cleans up any subscribed events.
    /// </summary>
    protected virtual void OnDestroy()
    {
        // Clear event subscribers to prevent memory leaks
        onInteractionCompleted = null;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Called when the player attempts to interact with this object.
    /// Override this method in derived classes to implement custom interaction behavior.
    /// </summary>
    /// <remarks>
    /// When overriding:
    /// - Call base.Interact() if you want to maintain the isInteracting state
    /// - Set isInteracting = false when the interaction is complete
    /// - Call CallInteractionCompletedEvent() to notify subscribers
    /// 
    /// Note: Derived classes should check canInteract if they want to prevent interaction
    /// when disabled.
    /// </remarks>
    public virtual void Interact()
    {
        isInteracting = true;
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Called to check if an interaction is completed.
    /// Override this method to implement custom completion logic.
    /// </summary>
    /// <param name="value">True if the interaction should be marked as completed</param>
    /// <remarks>
    /// This method is responsible for:
    /// - Updating the interaction state
    /// - Cleaning up any resources used during interaction
    /// - Preparing the object for the next interaction
    /// </remarks>
    protected virtual void IsInteractionCompleted(bool value)
    {
        if (!isInteracting) return;
        isInteracting = false;
    }

    /// <summary>
    /// Triggers the interaction completed event.
    /// Call this method when your interaction is finished to notify subscribers.
    /// </summary>
    /// <remarks>
    /// This method:
    /// - Safely invokes the onInteractionCompleted event
    /// - Handles null event handlers
    /// - Should be called after cleaning up the interaction state
    /// </remarks>
    protected virtual void CallInteractionCompletedEvent()
    {
        try
        {
            onInteractionCompleted?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error in interaction completed event: {e.Message}");
        }
    }

    /// <summary>
    /// Validates whether interaction can proceed.
    /// Helper method for derived classes to use in their Interact method.
    /// </summary>
    /// <returns>True if interaction can proceed, false otherwise</returns>
    protected virtual bool ValidateInteraction()
    {
        if (!canInteract)
        {
            Debug.LogWarning($"[{gameObject.name}] Attempted to interact while interaction is disabled");
            return false;
        }

        if (isInteracting)
        {
            Debug.LogWarning($"[{gameObject.name}] Attempted to interact while already interacting");
            return false;
        }

        return true;
    }
    #endregion
}
