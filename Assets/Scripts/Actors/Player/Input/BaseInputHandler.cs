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
            case InputActionType.UseSpell:
                ProcessUseSpellInput(context);
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
    protected abstract void ProcessUseSpellInput(InputAction.CallbackContext context);
    protected abstract void ProcessJumpInput(InputAction.CallbackContext context);
    protected abstract void ProcessDashInput(InputAction.CallbackContext context);
    protected abstract void ProcessInteractInput(InputAction.CallbackContext context);
    protected abstract void ProcessPlayerMenuInput(InputAction.CallbackContext context);
}
