using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles keyboard and mouse specific input processing
/// </summary>
public class KeyboardInputHandler : BaseInputHandler
{
    // playerInputHandler is now in the base class

    // Flags to track if inputs have been processed to prevent double-triggering
    // For keyboard, these prevent multiple activations due to key repeat rates
    // when a key is held down, which is different from the analog trigger issue on gamepads
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

    public KeyboardInputHandler(PlayerInputHandler playerInputHandler) : base(playerInputHandler)
    {
    }

    // ProcessInput is now implemented in the base class

    // Update is now implemented in the base class

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

    // Legacy method - will be replaced by individual slot methods
    [System.Obsolete("This method is deprecated. Use ProcessUseSpellSlot0, ProcessUseSpellSlot1, or ProcessUseSpellSlot2 instead.")]
    protected override void ProcessUseSpellInput(InputAction.CallbackContext context)
    {
        // Get the index of the triggered binding
        var bindingIndex = context.action.GetBindingIndexForControl(context.control);
        
        // Keyboard bindings map directly to hotbar slots
        int hotbarSlot = bindingIndex;
        
        if (context.started && !spellCastInputProcessed)
        {
            playerInputHandler.SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Activate the hotbar slot
            playerInputHandler.HotbarState.ActivateSlot(hotbarSlot);
            Debug.Log($"Keyboard spell activated: Hotbar {hotbarSlot}");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the key we're currently using
            if (playerInputHandler.HotbarState.IsSpellActive && 
                hotbarSlot == playerInputHandler.HotbarState.CurrentSlot)
            {
                playerInputHandler.SpellCastInput = false;
                
                // Deactivate the hotbar slot
                playerInputHandler.HotbarState.DeactivateSlot();

            }
            spellCastInputProcessed = false;
            return;
        }

        // Update the last used hotbar slot
        playerInputHandler.HotbarState.SetLastSlot(hotbarSlot);

    }
    
    // New individual spell slot methods
    protected override void ProcessUseSpellSlot0(InputAction.CallbackContext context)
    {
        ProcessSpellSlotInput(context, 0);
    }
    
    protected override void ProcessUseSpellSlot1(InputAction.CallbackContext context)
    {
        ProcessSpellSlotInput(context, 1);
    }
    
    protected override void ProcessUseSpellSlot2(InputAction.CallbackContext context)
    {
        ProcessSpellSlotInput(context, 2);
    }
    
    private void ProcessSpellSlotInput(InputAction.CallbackContext context, int slotIndex)
    {
        if (context.started && !spellCastInputProcessed)
        {
            playerInputHandler.SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Activate the hotbar slot
            playerInputHandler.HotbarState.ActivateSlot(slotIndex);

        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the key we're currently using
            if (playerInputHandler.HotbarState.IsSpellActive && 
                slotIndex == playerInputHandler.HotbarState.CurrentSlot)
            {
                playerInputHandler.SpellCastInput = false;
                
                // Deactivate the hotbar slot
                playerInputHandler.HotbarState.DeactivateSlot();

            }
            spellCastInputProcessed = false;
            return;
        }

        // Update the last used hotbar slot
        playerInputHandler.HotbarState.SetLastSlot(slotIndex);
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
}
