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
    }

    /// <summary>
    /// Checks if the LT modifier has been released while a spell is active
    /// </summary>
    private void CheckSpellModifierReleased()
    {
        // If we're casting a spell and the LT trigger is released, cancel the spell
        if (playerInputHandler.SpellCastInput && 
            playerInputHandler.UseSpellHotbarDictionary.ContainsKey(true) && 
            !SpellModifierActive)
        {
            Debug.Log("Gamepad LT trigger released while spell was active - canceling spell");
            playerInputHandler.SpellCastInput = false;
            
            // Update the dictionary with inactive state but keep the hotbar number
            int lastSlot = playerInputHandler.UseSpellHotbarDictionary[true];
            playerInputHandler.UseSpellHotbarDictionary[false] = lastSlot;
            playerInputHandler.UseSpellHotbarDictionary.Remove(true);
            
            // Reset the processed flag to allow new inputs
            spellCastInputProcessed = false;
        }
    }

    #region Input Processing Methods
    protected override void ProcessAttackInput(InputAction.CallbackContext context)
    {
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

    protected override void ProcessUseSpellInput(InputAction.CallbackContext context)
    {
        // Get the index of the triggered binding
        var bindingIndex = context.action.GetBindingIndexForControl(context.control);
        
        // Check if this is a gamepad binding index
        bool isGamepadInput = IsGamepadBindingIndex(bindingIndex);
        if (!isGamepadInput)
            return;
            
        // Map the binding index to the appropriate hotbar slot
        int hotbarSlot = MapGamepadBindingIndexToHotbarSlot(bindingIndex);
        
        // Debug the LT trigger value
        float ltValue = Gamepad.current != null ? Gamepad.current.leftTrigger.ReadValue() : 0f;
        Debug.Log($"LT Trigger Value: {ltValue}");
        
        if (context.started && !spellCastInputProcessed)
        {
            // For gamepad inputs, only activate if the modifier is active
            Debug.Log($"Gamepad spell button check - Button: {bindingIndex}, ModifierActive: {SpellModifierActive}, LT Value: {ltValue}");
            
            if (!SpellModifierActive)
            {
                Debug.Log($"Gamepad spell button pressed without modifier: {bindingIndex}");
                return; // Don't process this input without the modifier
            }
            
            playerInputHandler.SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Update the dictionary with active state and current hotbar number
            playerInputHandler.UseSpellHotbarDictionary[true] = hotbarSlot;
            Debug.Log($"Gamepad spell activated: Hotbar {hotbarSlot}, Active: {true}");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the button we're currently using
            if (playerInputHandler.UseSpellHotbarDictionary.ContainsKey(true) && 
                hotbarSlot == playerInputHandler.UseSpellHotbarDictionary[true])
            {
                playerInputHandler.SpellCastInput = false;
                
                // Update the dictionary with inactive state but keep the hotbar number
                playerInputHandler.UseSpellHotbarDictionary[false] = hotbarSlot;
                if (playerInputHandler.UseSpellHotbarDictionary.ContainsKey(true))
                {
                    playerInputHandler.UseSpellHotbarDictionary.Remove(true);
                }
                Debug.Log($"Gamepad spell deactivated: Hotbar {hotbarSlot}, Active: {false}");
            }
            spellCastInputProcessed = false;
            return;
        }

        // Only update the hotbar number if the modifier is active
        if (SpellModifierActive)
        {
            playerInputHandler.UseSpellHotbarDictionary[false] = hotbarSlot;
            Debug.Log($"Gamepad spell hotbar set to: {hotbarSlot}");
        }
        else
        {
            Debug.Log($"Ignoring hotbar update for gamepad button {bindingIndex} without modifier");            
        }
    }

    protected override void ProcessJumpInput(InputAction.CallbackContext context)
    {
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
    /// <summary>
    /// Determines if a binding index corresponds to a gamepad input
    /// </summary>
    private bool IsGamepadBindingIndex(int bindingIndex)
    {
        return bindingIndex >= GamepadControls.FirstButtonIndex && 
               bindingIndex <= GamepadControls.ThirdButtonIndex;
    }
    
    /// <summary>
    /// Maps a gamepad binding index to the appropriate hotbar slot
    /// </summary>
    private int MapGamepadBindingIndexToHotbarSlot(int bindingIndex)
    {
        // Map gamepad binding indices to specific slots
        switch (bindingIndex)
        {
            case GamepadControls.FirstButtonIndex: // Index 3
                return GamepadControls.FirstButtonSlot; // Maps to slot 0
            case GamepadControls.SecondButtonIndex: // Index 4
                return GamepadControls.SecondButtonSlot; // Maps to slot 0
            case GamepadControls.ThirdButtonIndex: // Index 5
                return GamepadControls.ThirdButtonSlot; // Maps to slot 1
            default: 
                return 0;
        }
    }
    #endregion
}
