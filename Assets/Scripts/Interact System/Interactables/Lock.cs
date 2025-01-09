using Opsive.UltimateInventorySystem.Core;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles lock functionality for key-based interactions.
/// Manages lock state persistence, key validation, and visual feedback.
/// Integrates with the Ultimate Inventory System for item checking.
/// </summary>
public class Lock : InteractableState
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Item definition required to unlock")]
    /// <summary>
    /// Item definition required to unlock this lock.
    /// Must match an item definition in the Ultimate Inventory System.
    /// </summary>
    private ItemDefinition m_itemDefinition;
    #endregion

    #region Private Fields
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

        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }
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

        var item = InventorySystemManager.CreateItem(m_itemDefinition);
        if (item == null)
        {
            Debug.LogError($"[{gameObject.name}] Failed to create item: {m_itemDefinition}");
            return;
        }

        if (Player.Instance.m_inventory.HasItem(item, false))
        {
            // Remove the key item from inventory
            Player.Instance.m_inventory.RemoveItem(item, 1);

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
    /// Initializes lock state from saved data.
    /// Disables interaction if lock was previously unlocked.
    /// </summary>
    private void InitializeLockState()
    {
        if (!ValidateComponents()) return;

        bool isUnlocked = InitializeStateFromLua();
        if (isUnlocked)
        {
            canInteract = false;
            m_animator.Play("UnlockedIdle");
        }
    }

    /// <summary>
    /// Validates that all required components are present and properly initialized.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    protected override bool ValidateComponents()
    {
        if (!base.ValidateComponents()) return false;

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
    /// Unlocks the lock and notifies connected gates
    /// </summary>
    private void UnlockAndNotify()
    {
        // First set our states
        canInteract = false; // Disable further interaction
        DialogueLua.SetVariable($"{StatePrefix}{m_stateID}", true);
        
        // Then notify each connected gate
        foreach (Gate gate in m_connectedGates)
        {
            if (gate != null)
            {
                string gateId = gate.m_gateID;
                
                // First set the gate state
                DialogueLua.SetVariable($"{StatePrefix}{gateId}", true);
                
                // Then notify the gate to trigger state change
                m_gameEvents.TriggerStateChanged(gateId);
            }
        }
    }
    #endregion
}
