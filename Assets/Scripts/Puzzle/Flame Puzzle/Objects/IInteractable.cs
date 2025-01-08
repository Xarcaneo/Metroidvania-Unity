using UnityEngine;

/// <summary>
/// Interface for objects that can be interacted with by the player
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    void Interact();

    /// <summary>
    /// Whether the object can currently be interacted with
    /// </summary>
    bool CanInteract { get; }

    /// <summary>
    /// Optional interaction prompt text to display
    /// </summary>
    string InteractionPrompt { get; }
}
