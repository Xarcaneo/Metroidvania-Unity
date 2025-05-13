using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles keyboard and mouse specific input processing
/// </summary>
public class KeyboardInputHandler : IInputHandler
{
    private PlayerInputHandler playerInputHandler;

    // Flags to track if inputs have been processed to prevent double-triggering
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

    public KeyboardInputHandler(PlayerInputHandler playerInputHandler)
    {
        this.playerInputHandler = playerInputHandler;
    }

    /// <summary>
    /// Process input from keyboard/mouse
    /// </summary>
    public void ProcessInput(InputAction.CallbackContext context, string actionName)
    {
        if (playerInputHandler.DisableInput)
            return;

        switch (actionName)
        {
            case "Attack":
                ProcessAttackInput(context);
                break;
            case "Block":
                ProcessBlockInput(context);
                break;
            case "HotbarAction":
                ProcessHotbarActionInput(context);
                break;
            case "ItemSwitchLeft":
                ProcessItemSwitchLeftInput(context);
                break;
            case "ItemSwitchRight":
                ProcessItemSwitchRightInput(context);
                break;
            case "Move":
                ProcessMoveInput(context);
                break;
            case "UseSpell":
                ProcessUseSpellInput(context);
                break;
            case "Jump":
                ProcessJumpInput(context);
                break;
            case "Dash":
                ProcessDashInput(context);
                break;
            case "Interact":
                ProcessInteractInput(context);
                break;
            case "PlayerMenu":
                ProcessPlayerMenuInput(context);
                break;
        }
    }

    /// <summary>
    /// Update method called every frame
    /// </summary>
    public void Update()
    {
        // No specific keyboard update logic needed
    }

    #region Input Processing Methods
    private void ProcessAttackInput(InputAction.CallbackContext context)
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

    private void ProcessBlockInput(InputAction.CallbackContext context)
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

    private void ProcessHotbarActionInput(InputAction.CallbackContext context)
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

    private void ProcessItemSwitchLeftInput(InputAction.CallbackContext context)
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

    private void ProcessItemSwitchRightInput(InputAction.CallbackContext context)
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

    private void ProcessMoveInput(InputAction.CallbackContext context)
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

    private void ProcessUseSpellInput(InputAction.CallbackContext context)
    {
        // Get the index of the triggered binding
        var bindingIndex = context.action.GetBindingIndexForControl(context.control);
        
        // Keyboard bindings map directly to hotbar slots
        int hotbarSlot = bindingIndex;
        
        if (context.started && !spellCastInputProcessed)
        {
            playerInputHandler.SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Update the dictionary with active state and current hotbar number
            playerInputHandler.UseSpellHotbarDictionary[true] = hotbarSlot;
            Debug.Log($"Keyboard spell activated: Hotbar {hotbarSlot}, Active: {true}");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the key we're currently using
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
                Debug.Log($"Keyboard spell deactivated: Hotbar {hotbarSlot}, Active: {false}");
            }
            spellCastInputProcessed = false;
            return;
        }

        // Update the hotbar number
        playerInputHandler.UseSpellHotbarDictionary[false] = hotbarSlot;
        Debug.Log($"Keyboard spell hotbar set to: {hotbarSlot}");
    }

    private void ProcessJumpInput(InputAction.CallbackContext context)
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

    private void ProcessDashInput(InputAction.CallbackContext context)
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

    private void ProcessInteractInput(InputAction.CallbackContext context)
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

    private void ProcessPlayerMenuInput(InputAction.CallbackContext context)
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
