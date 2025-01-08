using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all player input using the new Input System.
/// Processes raw input data and provides normalized values for movement and actions.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    #region Components
    private PlayerInput playerInput;
    #endregion

    #region Input Properties
    /// <summary>
    /// Raw movement input vector from input device
    /// </summary>
    public Vector2 RawMovementInput { get; private set; }

    /// <summary>
    /// Flag to disable all input processing
    /// </summary>
    public bool DisableInput = false;

    /// <summary>
    /// Normalized X-axis input (-1, 0, 1)
    /// </summary>
    public int NormInputX { get; private set; }

    /// <summary>
    /// Normalized Y-axis input (-1, 0, 1)
    /// </summary>
    public int NormInputY { get; private set; }

    /// <summary>
    /// Jump button pressed state
    /// </summary>
    public bool JumpInput { get; private set; }

    /// <summary>
    /// Jump button released state
    /// </summary>
    public bool JumpInputStop { get; private set; }

    /// <summary>
    /// Action button (dash) pressed state
    /// </summary>
    public bool ActionInput { get; private set; }

    /// <summary>
    /// Attack button pressed state
    /// </summary>
    public bool AttackInput { get; private set; }

    /// <summary>
    /// Block button pressed state
    /// </summary>
    public bool BlockInput { get; private set; }

    /// <summary>
    /// Hotbar action button pressed state
    /// </summary>
    public bool HotbarActionInput { get; private set; }

    /// <summary>
    /// Item switch left button pressed state
    /// </summary>
    public bool ItemSwitchLeftInput { get; private set; }

    /// <summary>
    /// Item switch right button pressed state
    /// </summary>
    public bool ItemSwitchRightInput { get; private set; }
    #endregion

    #region Input Settings
    [SerializeField, Tooltip("Maximum time to hold input before it's cleared")]
    private float inputHoldTime = 0.2f;
    #endregion

    #region Input Timers
    private float jumpInputStartTime;
    private float actionInputInputStartTime;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }
    #endregion

    #region Event Management
    private void SubscribeToEvents()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger += EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger += EnableDisablePlayerInput;
            GameEvents.Instance.onDeactivatePlayerInput += EnableDisablePlayerInput;
        }
        catch
        {
            Debug.LogWarning("Failed to subscribe to game events. GameEvents instance might not be available.");
        }
    }

    private void UnsubscribeFromEvents()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger -= EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger -= EnableDisablePlayerInput;
            GameEvents.Instance.onDeactivatePlayerInput -= EnableDisablePlayerInput;
        }
        catch
        {
            Debug.LogWarning("Failed to unsubscribe from game events. GameEvents instance might not be available.");
        }
    }
    #endregion

    #region Input System Callbacks
    /// <summary>
    /// Handles attack input events from the Input System
    /// </summary>
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                AttackInput = true;
            }
            else if (context.canceled)
            {
                AttackInput = false;
            }
        }
    }

    /// <summary>
    /// Handles block input events from the Input System
    /// </summary>
    public void OnBlockInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                BlockInput = true;
            }
            else if (context.canceled)
            {
                BlockInput = false;
            }
        }
    }

    /// <summary>
    /// Handles hotbar action input events from the Input System
    /// </summary>
    public void OnHotbarActionInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                HotbarActionInput = true;
            }
            else if (context.canceled)
            {
                HotbarActionInput = false;
            }
        }
    }

    /// <summary>
    /// Handles item switch left input events from the Input System
    /// </summary>
    public void OnItemSwitchLeftInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                ItemSwitchLeftInput = true;
            }
            else if (context.canceled)
            {
                ItemSwitchLeftInput = false;
            }
        }
    }

    /// <summary>
    /// Handles item switch right input events from the Input System
    /// </summary>
    public void OnItemSwitchRightInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                ItemSwitchRightInput = true;
            }
            else if (context.canceled)
            {
                ItemSwitchRightInput = false;
            }
        }
    }

    /// <summary>
    /// Handles movement input events from the Input System
    /// </summary>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            RawMovementInput = context.ReadValue<Vector2>();
            NormInputX = Mathf.RoundToInt(RawMovementInput.x);

            if (Mathf.Abs(RawMovementInput.y) > 0.5f)
            {
                NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
            }
            else
            {
                NormInputY = 0;
            }
        }
    }

    /// <summary>
    /// Handles jump input events from the Input System
    /// </summary>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                JumpInput = true;
                JumpInputStop = false;
                jumpInputStartTime = Time.time;
            }
            else if (context.canceled)
            {
                JumpInputStop = true;
            }
        }
    }

    /// <summary>
    /// Handles dash input events from the Input System
    /// </summary>
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                ActionInput = true;
                actionInputInputStartTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Handles interact input events from the Input System
    /// </summary>
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            GameEvents.Instance.InteractTrigger(context.started);
        }
    }
    #endregion

    #region Input Consumption Methods
    /// <summary>
    /// Consumes the jump input
    /// </summary>
    public void UseJumpInput() => JumpInput = false;

    /// <summary>
    /// Consumes the dash input
    /// </summary>
    public void UseDashInput() => ActionInput = false;

    /// <summary>
    /// Consumes the attack input
    /// </summary>
    public void UseAttackInput() => AttackInput = false;

    /// <summary>
    /// Consumes the block input
    /// </summary>
    public void UseBlockInput() => BlockInput = false;

    /// <summary>
    /// Consumes the hotbar action input
    /// </summary>
    public void UseHotbarActionInput() => HotbarActionInput = false;

    /// <summary>
    /// Consumes the item switch left input
    /// </summary>
    public void UseItemSwitchLeftInput() => ItemSwitchLeftInput = false;

    /// <summary>
    /// Consumes the item switch right input
    /// </summary>
    public void UseItemSwitchRightInput() => ItemSwitchRightInput = false;
    #endregion

    #region Input Check Methods
    /// <summary>
    /// Checks if jump input has been held longer than the maximum hold time
    /// </summary>
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    /// <summary>
    /// Checks if dash input has been held longer than the maximum hold time
    /// </summary>
    private void CheckDashInputHoldTime()
    {
        if (Time.time >= actionInputInputStartTime + inputHoldTime)
        {
            ActionInput = false;
        }
    }
    #endregion

    #region Input State Management
    /// <summary>
    /// Enables or disables all player input
    /// </summary>
    /// <param name="disable">True to disable input, false to enable</param>
    private void EnableDisablePlayerInput(bool disable)
    {
        DisableInput = disable;
        if (disable)
        {
            NormInputX = 0;
            NormInputY = 0;
        }
    }
    #endregion
}