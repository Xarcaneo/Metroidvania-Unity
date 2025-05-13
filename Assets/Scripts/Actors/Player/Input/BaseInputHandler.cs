using UnityEngine.InputSystem;

/// <summary>
/// Base class for input handlers that provides common functionality
/// </summary>
public abstract class BaseInputHandler : IInputHandler
{
    protected PlayerInputHandler playerInputHandler;

    protected BaseInputHandler(PlayerInputHandler playerInputHandler)
    {
        this.playerInputHandler = playerInputHandler;
    }

    /// <summary>
    /// Process input from any input device
    /// </summary>
    public virtual void ProcessInput(InputAction.CallbackContext context, InputActionType actionType)
    {
        if (playerInputHandler.DisableInput)
            return;

        switch (actionType)
        {
            case InputActionType.Attack:
                ProcessAttackInput(context);
                break;
            case InputActionType.Block:
                ProcessBlockInput(context);
                break;
            case InputActionType.HotbarAction:
                ProcessHotbarActionInput(context);
                break;
            case InputActionType.ItemSwitchLeft:
                ProcessItemSwitchLeftInput(context);
                break;
            case InputActionType.ItemSwitchRight:
                ProcessItemSwitchRightInput(context);
                break;
            case InputActionType.Move:
                ProcessMoveInput(context);
                break;
#pragma warning disable CS0618 // Disable obsolete warning for legacy UseSpell action
            case InputActionType.UseSpell: // Legacy method, will be replaced by slot-specific actions
                // For backward compatibility, map to slot 0
                ProcessUseSpellSlot0(context);
                break;
#pragma warning restore CS0618 // Restore obsolete warning
            case InputActionType.UseSpellSlot0:
                ProcessUseSpellSlot0(context);
                break;
            case InputActionType.UseSpellSlot1:
                ProcessUseSpellSlot1(context);
                break;
            case InputActionType.UseSpellSlot2:
                ProcessUseSpellSlot2(context);
                break;
            case InputActionType.Jump:
                ProcessJumpInput(context);
                break;
            case InputActionType.Dash:
                ProcessDashInput(context);
                break;
            case InputActionType.Interact:
                ProcessInteractInput(context);
                break;
            case InputActionType.PlayerMenu:
                ProcessPlayerMenuInput(context);
                break;
        }
    }

    /// <summary>
    /// Update method called every frame
    /// </summary>
    public virtual void Update() { }

    // Protected abstract methods that derived classes must implement
    protected abstract void ProcessAttackInput(InputAction.CallbackContext context);
    protected abstract void ProcessBlockInput(InputAction.CallbackContext context);
    protected abstract void ProcessHotbarActionInput(InputAction.CallbackContext context);
    protected abstract void ProcessItemSwitchLeftInput(InputAction.CallbackContext context);
    protected abstract void ProcessItemSwitchRightInput(InputAction.CallbackContext context);
    protected abstract void ProcessMoveInput(InputAction.CallbackContext context);
    // Legacy method - now maps to slot 0 for backward compatibility
    [System.Obsolete("This method is deprecated. Override ProcessUseSpellSlot0, ProcessUseSpellSlot1, or ProcessUseSpellSlot2 instead.")]
    protected virtual void ProcessUseSpellInput(InputAction.CallbackContext context)
    {
        // By default, map to slot 0 for backward compatibility
        ProcessUseSpellSlot0(context);
    }
    
    // New spell slot methods
    protected virtual void ProcessUseSpellSlot0(InputAction.CallbackContext context) { }
    protected virtual void ProcessUseSpellSlot1(InputAction.CallbackContext context) { }
    protected virtual void ProcessUseSpellSlot2(InputAction.CallbackContext context) { }
    
    protected abstract void ProcessJumpInput(InputAction.CallbackContext context);
    protected abstract void ProcessDashInput(InputAction.CallbackContext context);
    protected abstract void ProcessInteractInput(InputAction.CallbackContext context);
    protected abstract void ProcessPlayerMenuInput(InputAction.CallbackContext context);
}
