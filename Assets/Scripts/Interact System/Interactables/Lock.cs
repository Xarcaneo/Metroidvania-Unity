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
    [Tooltip("Name of the key item required to unlock")]
    /// <summary>
    /// Name of the key item required to unlock this lock.
    /// Must match an item name in the Ultimate Inventory System.
    /// </summary>
    private string m_itemName;
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
    /// Initializes lock state from saved data.
    /// Disables interaction if lock was previously unlocked.
    /// </summary>
    private void InitializeLockState()
    {
        try
        {
            var lockState = DialogueLua.GetVariable($"{StatePrefix}{m_stateID}").asBool;
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
    protected override bool ValidateComponents()
    {
        if (!base.ValidateComponents()) return false;
        
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

        // Start monitoring gate events
        StartCoroutine(CheckConnectedGatesEventCompletion());
    }

    /// <summary>
    /// Monitors connected gates for event completion.
    /// Continues checking until all gates have completed their events.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator CheckConnectedGatesEventCompletion()
    {
        if (m_connectedGates == null || m_connectedGates.Count == 0)
        {
            CallInteractionCompletedEvent();
            yield break;
        }

        while (true)
        {
            bool allGatesCompleted = true;

            foreach (Gate gate in m_connectedGates)
            {
                if (gate == null)
                {
                    Debug.LogError($"[{gameObject.name}] Connected gate is null!");
                    continue;
                }

                if (!gate.isEventCompleted)
                {
                    allGatesCompleted = false;
                    break;
                }
            }

            if (allGatesCompleted)
            {
                CallInteractionCompletedEvent();
                yield break;
            }

            yield return null;
        }
    }
    #endregion
}
