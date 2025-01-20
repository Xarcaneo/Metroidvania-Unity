using UnityEngine;

/// <summary>
/// Base class for handling area-based events in the game.
/// Provides common functionality for trigger-based events, such as layer filtering,
/// cooldowns, and one-shot behavior.
/// Derived classes must implement the specific event logic in the <see cref="Execute"/> method.
/// </summary>
public abstract class BaseAreaEvent : MonoBehaviour
{
    [Header("Base Event Settings")]

    /// <summary>
    /// The layer mask to determine which objects can trigger the event.
    /// </summary>
    [SerializeField]
    private LayerMask targetLayer;

    /// <summary>
    /// Indicates whether the event should be triggered only once.
    /// If true, the GameObject will be destroyed after the event is triggered.
    /// </summary>
    [SerializeField]
    private bool isOneShot = true;

    /// <summary>
    /// The cooldown time in seconds before the event can be triggered again.
    /// </summary>
    [SerializeField]
    private float cooldownTime = 0f;

    /// <summary>
    /// Flag indicating whether the event is currently in cooldown and cannot be triggered.
    /// </summary>
    private bool isInCooldown = false;

    /// <summary>
    /// Unity method called when another collider enters the trigger area.
    /// Executes the event if the collider matches the target layer and cooldown requirements.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger area.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsLayerMatch(collision)) return;
        if (isInCooldown) return;

        Execute(collision);

        if (cooldownTime > 0f)
        {
            StartCooldown();
        }

        if (isOneShot)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Determines if the given collider's layer matches the specified target layer mask.
    /// </summary>
    /// <param name="collider">The collider to check against the target layer mask.</param>
    /// <returns>True if the collider's layer matches the target layer mask; otherwise, false.</returns>
    private bool IsLayerMatch(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & targetLayer) != 0;
    }

    /// <summary>
    /// Starts the cooldown period, during which the event cannot be triggered.
    /// </summary>
    private void StartCooldown()
    {
        isInCooldown = true;
        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    /// <summary>
    /// Resets the cooldown, allowing the event to be triggered again.
    /// </summary>
    private void ResetCooldown()
    {
        isInCooldown = false;
    }

    /// <summary>
    /// Executes the specific logic for the event.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <param name="collider">The collider that triggered the event.</param>
    protected abstract void Execute(Collider2D collider);
}
