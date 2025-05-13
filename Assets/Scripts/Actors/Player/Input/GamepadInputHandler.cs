using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Handles gamepad specific input processing
/// </summary>
public class GamepadInputHandler : BaseInputHandler
{
    // playerInputHandler is now in the base class

    // Flags to track if inputs have been processed to prevent double-triggering with analog triggers
    private bool actionInputProcessed = false;
    private bool attackInputProcessed = false;
    private bool blockInputProcessed = false;
    private bool hotbarActionInputProcessed = false;
    private bool itemSwitchLeftInputProcessed = false;
    private bool itemSwitchRightInputProcessed = false;
    private bool jumpInputProcessed = false;
    private bool spellCastInputProcessed = false;
    private bool interactInputProcessed = false;
    private bool playerMenuInputProcessed = false;

    /// <summary> 
    /// Flag to track if the LT modifier is being held for gamepad spell casting
    /// </summary>
    private bool SpellModifierActive => Gamepad.current != null && Gamepad.current.leftTrigger.ReadValue() > GamepadControls.TriggerThreshold;

    public GamepadInputHandler(PlayerInputHandler playerInputHandler) : base(playerInputHandler)
    {
    }

    // ProcessInput is now implemented in the base class

    /// <summary>
    /// Update method called every frame
    /// </summary>
    public override void Update()
    {
        CheckSpellModifierReleased();
        UpdateSpellCastingPriority();
    }
    
    /// <summary>
    /// Updates the spell casting priority based on the modifier state
    /// </summary>
    private void UpdateSpellCastingPriority()
    {
        // Check if the LT modifier is active
        bool modifierActive = SpellModifierActive;
        
        // Set priority directly based on modifier state - no cooldown
        prioritizeSpellCasting = modifierActive;
        
        // Debug only when state changes
        if (prioritizeSpellCasting != lastPrioritizeSpellCasting)
        {
            Debug.Log($"Spell casting priority: {prioritizeSpellCasting} (Modifier active: {modifierActive})");
            lastPrioritizeSpellCasting = prioritizeSpellCasting;
        }
    }
    
    private bool lastPrioritizeSpellCasting = false;
    
    /// <summary>
    /// Flag to track if we should prioritize spell casting over other actions
    /// </summary>
    private bool prioritizeSpellCasting = false;
    
    // Removed spellModifierActivationTime and priority cooldown to allow for immediate switching between spell casting and other actions

    /// <summary>
    /// Converts an InputActionType to the corresponding hotbar slot index
    /// </summary>
    private int GetSlotIndexFromActionType(InputActionType actionType)
    {
        switch (actionType)
        {
            case InputActionType.UseSpellSlot0:
                return 0;
            case InputActionType.UseSpellSlot1:
                return 1;
            case InputActionType.UseSpellSlot2:
                return 2;
            default:
                Debug.LogError($"Unexpected action type for spell slot: {actionType}");
                throw new System.ArgumentException($"Invalid action type for spell slot: {actionType}");
        }
    }

    /// <summary>
    /// Checks if the LT modifier has been released while a spell is active
    /// </summary>
    private void CheckSpellModifierReleased()
    {
        // If we're casting a spell and the LT trigger is released, cancel the spell
        if (playerInputHandler.SpellCastInput && 
            playerInputHandler.HotbarState.IsSpellActive && 
            !SpellModifierActive)
        {
            Debug.Log("Gamepad LT trigger released while spell was active - canceling spell");
            playerInputHandler.SpellCastInput = false;
            
            // Deactivate the hotbar slot
            playerInputHandler.HotbarState.DeactivateSlot();
            
            // Reset the processed flag to allow new inputs
            spellCastInputProcessed = false;
        }
    }

    #region Input Processing Methods
    protected override void ProcessAttackInput(InputAction.CallbackContext context)
    {
        // Check the current state of the spell modifier
        bool modifierActive = SpellModifierActive;
        
        // If we're prioritizing spell casting, ignore attack input
        if (modifierActive || prioritizeSpellCasting)
        {
            Debug.Log($"Ignoring attack input due to spell casting priority (Modifier: {modifierActive}, Priority: {prioritizeSpellCasting})");
            return;
        }
        
        if (context.started && !attackInputProcessed)
        {
            playerInputHandler.AttackInput = true;
            attackInputProcessed = true;
        }
        else if (context.canceled)
        {
            playerInputHandler.AttackInput = false;
            attackInputProcessed = false;
        }
    }

    protected override void ProcessBlockInput(InputAction.CallbackContext context)
    {
        // Check the current state of the spell modifier
        bool modifierActive = SpellModifierActive;
        
        // If we're prioritizing spell casting, ignore block input
        if (modifierActive || prioritizeSpellCasting)
        {
            Debug.Log($"Ignoring block input due to spell casting priority (Modifier: {modifierActive}, Priority: {prioritizeSpellCasting})");
            return;
        }
        
        if (context.started && !blockInputProcessed)
        {
            playerInputHandler.BlockInput = true;
            blockInputProcessed = true;
        }
        else if (context.canceled)
        {
            playerInputHandler.BlockInput = false;
            blockInputProcessed = false;
        }
    }

    protected override void ProcessHotbarActionInput(InputAction.CallbackContext context)
    {
        if (context.started && !hotbarActionInputProcessed)
        {
            playerInputHandler.HotbarActionInput = true;
            hotbarActionInputProcessed = true;
        }
        else if (context.canceled)
        {
            playerInputHandler.HotbarActionInput = false;
            hotbarActionInputProcessed = false;
        }
    }

    protected override void ProcessItemSwitchLeftInput(InputAction.CallbackContext context)
    {
        if (context.started && !itemSwitchLeftInputProcessed)
        {
            playerInputHandler.ItemSwitchLeftInput = true;
            itemSwitchLeftInputProcessed = true;
        }
        else if (context.canceled)
        {
            playerInputHandler.ItemSwitchLeftInput = false;
            itemSwitchLeftInputProcessed = false;
        }
    }

    protected override void ProcessItemSwitchRightInput(InputAction.CallbackContext context)
    {
        if (context.started && !itemSwitchRightInputProcessed)
        {
            playerInputHandler.ItemSwitchRightInput = true;
            itemSwitchRightInputProcessed = true;
        }
        else if (context.canceled)
        {
            playerInputHandler.ItemSwitchRightInput = false;
            itemSwitchRightInputProcessed = false;
        }
    }

    protected override void ProcessMoveInput(InputAction.CallbackContext context)
    {
        Vector2 rawMovementInput = context.ReadValue<Vector2>();
        playerInputHandler.RawMovementInput = rawMovementInput;
        playerInputHandler.NormInputX = Mathf.RoundToInt(rawMovementInput.x);

        if (Mathf.Abs(rawMovementInput.y) > 0.5f)
        {
            playerInputHandler.NormInputY = (int)(rawMovementInput * Vector2.up).normalized.y;
        }
        else
        {
            playerInputHandler.NormInputY = 0;
        }
    }

    // Replace the single ProcessUseSpellInput with separate methods for each slot
    
    /// <summary>
    /// Processes input for spell slot 0 (first slot)
    /// </summary>
    protected override void ProcessUseSpellSlot0(InputAction.CallbackContext context)
    {
        ProcessSpellSlotInput(context, InputActionType.UseSpellSlot0);
    }
    
    /// <summary>
    /// Processes input for spell slot 1 (second slot)
    /// </summary>
    protected override void ProcessUseSpellSlot1(InputAction.CallbackContext context)
    {
        ProcessSpellSlotInput(context, InputActionType.UseSpellSlot1);
    }
    
    /// <summary>
    /// Processes input for spell slot 2 (third slot)
    /// </summary>
    protected override void ProcessUseSpellSlot2(InputAction.CallbackContext context)
    {
        ProcessSpellSlotInput(context, InputActionType.UseSpellSlot2);
    }
    
    /// <summary>
    /// Legacy method for processing spell input - will be replaced by individual slot methods
    /// </summary>
    [System.Obsolete("This method is deprecated. Use ProcessUseSpellSlot0, ProcessUseSpellSlot1, or ProcessUseSpellSlot2 instead.")]
    protected override void ProcessUseSpellInput(InputAction.CallbackContext context)
    {
        // This is a legacy method that will be replaced by the individual slot methods
        // For backward compatibility, we'll handle it directly with slot index 0
        
        // Debug the LT trigger value, priority state, and input details
        float ltValue = Gamepad.current != null ? Gamepad.current.leftTrigger.ReadValue() : 0f;
        int slotIndex = 0; // Legacy method always maps to slot 0
        
        Debug.Log($"Legacy spell method - Slot {slotIndex} - LT Trigger Value: {ltValue}, Priority: {prioritizeSpellCasting}");
        Debug.Log($"Input details - Phase: {context.phase}, Control: {context.control?.path}, Action: {context.action?.name}, ActionType: Legacy");
        
        if (context.started && !spellCastInputProcessed)
        {
            // For gamepad inputs, only activate if we have priority (LT is active)
            if (!prioritizeSpellCasting)
            {
                Debug.Log($"Legacy spell method - Slot {slotIndex} pressed without priority");
                return; // Don't process this input without priority
            }
            
            playerInputHandler.SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Activate the hotbar slot
            playerInputHandler.HotbarState.ActivateSlot(slotIndex);
            Debug.Log($"Legacy spell method - Hotbar {slotIndex} activated");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the slot we're currently using
            if (playerInputHandler.HotbarState.IsSpellActive && 
                slotIndex == playerInputHandler.HotbarState.CurrentSlot)
            {
                playerInputHandler.SpellCastInput = false;
                
                // Deactivate the hotbar slot
                playerInputHandler.HotbarState.DeactivateSlot();
                Debug.Log($"Legacy spell method - Hotbar {slotIndex} deactivated");
            }
            spellCastInputProcessed = false;
        }

        // Only update the hotbar number if we have priority
        if (prioritizeSpellCasting)
        {
            playerInputHandler.HotbarState.SetLastSlot(slotIndex);
            Debug.Log($"Legacy spell method - Hotbar set to: {slotIndex}");
        }
    }
    
    /// <summary>
    /// Common method to handle spell slot input for a specific action type
    /// </summary>
    private void ProcessSpellSlotInput(InputAction.CallbackContext context, InputActionType actionType)
    {
        // Convert action type to slot index
        int slotIndex = GetSlotIndexFromActionType(actionType);
        
        // Debug the LT trigger value, priority state, and input details
        float ltValue = Gamepad.current != null ? Gamepad.current.leftTrigger.ReadValue() : 0f;
        Debug.Log($"Spell slot {slotIndex} - LT Trigger Value: {ltValue}, Priority: {prioritizeSpellCasting}");
        Debug.Log($"Input details - Phase: {context.phase}, Control: {context.control?.path}, Action: {context.action?.name}, ActionType: {actionType}");
        
        if (context.started && !spellCastInputProcessed)
        {
            // For gamepad inputs, only activate if we have priority (LT is active)
            Debug.Log($"Gamepad spell slot {slotIndex} check - Priority: {prioritizeSpellCasting}, LT Value: {ltValue}");
            
            if (!prioritizeSpellCasting)
            {
                Debug.Log($"Gamepad spell slot {slotIndex} pressed without priority");
                return; // Don't process this input without priority
            }
            
            playerInputHandler.SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Activate the hotbar slot
            playerInputHandler.HotbarState.ActivateSlot(slotIndex);
            Debug.Log($"Gamepad spell activated: Hotbar {slotIndex}");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the slot we're currently using
            if (playerInputHandler.HotbarState.IsSpellActive && 
                slotIndex == playerInputHandler.HotbarState.CurrentSlot)
            {
                playerInputHandler.SpellCastInput = false;
                
                // Deactivate the hotbar slot
                playerInputHandler.HotbarState.DeactivateSlot();
                Debug.Log($"Gamepad spell deactivated: Hotbar {slotIndex}");
            }
            spellCastInputProcessed = false;
        }

        // Only update the hotbar number if we have priority
        if (prioritizeSpellCasting)
        {
            playerInputHandler.HotbarState.SetLastSlot(slotIndex);
            Debug.Log($"Gamepad spell hotbar set to: {slotIndex}");
        }
        else
        {
            Debug.Log($"Ignoring hotbar update for slot {slotIndex} without priority");            
        }
    }

    protected override void ProcessJumpInput(InputAction.CallbackContext context)
    {
        // Check the current state of the spell modifier
        bool modifierActive = SpellModifierActive;
        
        // If we're prioritizing spell casting, ignore jump input
        if (modifierActive || prioritizeSpellCasting)
        {
            Debug.Log($"Ignoring jump input due to spell casting priority (Modifier: {modifierActive}, Priority: {prioritizeSpellCasting})");
            return;
        }
        
        if (context.started && !jumpInputProcessed)
        {
            playerInputHandler.JumpInput = true;
            playerInputHandler.JumpInputStop = false;
            playerInputHandler.SetJumpInputStartTime(Time.time);
            jumpInputProcessed = true;
        }
        else if (context.canceled)
        {
            playerInputHandler.JumpInputStop = true;
            jumpInputProcessed = false;
        }
    }

    protected override void ProcessDashInput(InputAction.CallbackContext context)
    {
        // Check the current state of the spell modifier
        bool modifierActive = SpellModifierActive;
        
        // If we're prioritizing spell casting, ignore dash input
        if (modifierActive || prioritizeSpellCasting)
        {
            Debug.Log($"Ignoring dash input due to spell casting priority (Modifier: {modifierActive}, Priority: {prioritizeSpellCasting})");
            return;
        }
        
        if (context.started && !actionInputProcessed)
        {
            playerInputHandler.ActionInput = true;
            actionInputProcessed = true;
            playerInputHandler.SetActionInputStartTime(Time.time);
        }
        else if (context.canceled)
        {
            playerInputHandler.ActionInput = false;
            actionInputProcessed = false;
        }
    }

    protected override void ProcessInteractInput(InputAction.CallbackContext context)
    {
        // Check the current state of the spell modifier
        bool modifierActive = SpellModifierActive;
        
        // If we're prioritizing spell casting, ignore interact input
        if (modifierActive || prioritizeSpellCasting)
        {
            Debug.Log($"Ignoring interact input due to spell casting priority (Modifier: {modifierActive}, Priority: {prioritizeSpellCasting})");
            return;
        }
        
        if (context.started && !interactInputProcessed)
        {
            GameEvents.Instance.InteractTrigger(true);
            interactInputProcessed = true;
        }
        else if (context.canceled)
        {
            GameEvents.Instance.InteractTrigger(false);
            interactInputProcessed = false;
        }
    }

    protected override void ProcessPlayerMenuInput(InputAction.CallbackContext context)
    {
        if (!playerInputHandler.IsDialogueActive)
        {
            if (context.started && !playerMenuInputProcessed)
            {
                GameEvents.Instance.PlayerMenuOpen();
                playerMenuInputProcessed = true;
            }
            else if (context.canceled)
            {
                playerMenuInputProcessed = false;
            }
        }
    }
    #endregion

    #region Helper Methods
    #endregion
}
