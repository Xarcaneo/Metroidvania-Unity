using Opsive.UltimateInventorySystem.Core;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles lock functionality for key-based interactions.
/// Manages lock state persistence, key validation, and visual feedback.
/// Integrates with the Ultimate Inventory System for item checking.
/// </summary>
public class Lock : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique ID for this lock")]
    /// <summary>
    /// Unique identifier for this lock.
    /// Used to track and persist lock state between game sessions.
    /// </summary>
    private string m_lockID;

    [SerializeField]
    [Tooltip("Name of the key item required to unlock")]
    /// <summary>
    /// Name of the key item required to unlock this lock.
    /// Must match an item name in the Ultimate Inventory System.
    /// </summary>
    private string m_itemName;

    [SerializeField]
    [Tooltip("Reference to the animator component")]
    /// <summary>
    /// Reference to the animator component for lock animations.
    /// Controls feedback animations like "no key" state.
    /// </summary>
    private Animator m_animator;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to game events system for trigger state changes.
    /// Cached for efficient access.
    /// </summary>
    private GameEvents m_gameEvents;

    // Animation state names
    /// <summary>
    /// Constants for animation state names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string NO_KEY_ANIM = "NoKeyAnimation";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates lock configuration in the Unity Editor.
    /// Ensures critical parameters are properly set.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        // Only warn if item name is null, empty string is valid
        if (m_itemName == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Item name is not set!");
        }

        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Initializes the lock by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes lock state after all objects are initialized.
    /// Waits for end of frame to ensure proper initialization order.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InitializeLockState();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the lock when activated by the player.
    /// Checks for required key item and updates lock state accordingly.
    /// </summary>
    public override void Interact()
    {
        if (!ValidateComponents()) return;

        var item = InventorySystemManager.CreateItem(m_itemName);
        if (item == null)
        {
            Debug.LogError($"[{gameObject.name}] Failed to create item: {m_itemName}");
            return;
        }

        if (Player.Instance.m_inventory.HasItem(item, false))
        {
            UnlockAndNotify();
        }
        else
        {
            m_animator.Play(NO_KEY_ANIM);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components.
    /// Called during Awake to ensure early component access.
    /// </summary>
    private void InitializeComponents()
    {
        // Get animator if not already assigned
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        // Get game events
        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    /// <summary>
    /// Initializes lock state from saved data.
    /// Disables interaction if lock was previously unlocked.
    /// </summary>
    private void InitializeLockState()
    {
        try
        {
            var lockState = DialogueLua.GetVariable($"Trigger.{m_lockID}").asBool;
            if (lockState)
            {
                canInteract = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error getting lock state: {e.Message}");
        }
    }

    /// <summary>
    /// Validates that all required components are present and properly initialized.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_itemName == null)
        {
            Debug.LogError($"[{gameObject.name}] Item name is not set!");
            return false;
        }

        // Only check animator if we need to play an animation
        if (m_animator == null && !string.IsNullOrEmpty(NO_KEY_ANIM))
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        if (Player.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] Player instance is null!");
            return false;
        }

        if (Player.Instance.m_inventory == null)
        {
            Debug.LogError($"[{gameObject.name}] Player inventory is null!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates lock state and notifies the game system of the change.
    /// Called when the correct key is used on the lock.
    /// </summary>
    private void UnlockAndNotify()
    {
        try
        {
            DialogueLua.SetVariable($"Trigger.{m_lockID}", true);
            if (m_gameEvents != null)
            {
                m_gameEvents.TriggerStateChanged(m_lockID);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error unlocking: {e.Message}");
        }
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when lock animation finishes.
    /// Notifies the interaction system that the interaction is complete.
    /// </summary>
    private void OnAnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
    #endregion
}
