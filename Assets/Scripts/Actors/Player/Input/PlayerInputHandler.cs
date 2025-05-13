using System;
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

    private bool isDialogueActive = false;

    // Flags to track if inputs have been processed to prevent double-triggering with analog triggers
    private bool actionInputProcessed = false;
    private bool attackInputProcessed = false;
    private bool blockInputProcessed = false;
    private bool hotbarActionInputProcessed = false;
    private bool itemSwitchLeftInputProcessed = false;
    private bool itemSwitchRightInputProcessed = false;
    private bool jumpInputProcessed = false;

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

    /// <summary>
    /// Hotbar number currently triggered.
    /// </summary>
    public int UseSpellHotbarNumber { get; private set; }

    /// <summary>
    /// Flag indicating if spell cast button is pressed
    /// </summary>
    /// <remarks>
    /// Set when any spell input is triggered. Used in conjunction with UseSpellType
    /// to determine which spell to cast.
    /// </remarks>
    public bool SpellCastInput { get; private set; }
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
            GameEvents.Instance.onDialogueTrigger += OnDialogueTrigger;
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
            GameEvents.Instance.onDialogueTrigger -= OnDialogueTrigger;

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
            if (context.started && !attackInputProcessed)
            {
                AttackInput = true;
                attackInputProcessed = true;
            }
            else if (context.canceled)
            {
                AttackInput = false;
                attackInputProcessed = false;
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
            if (context.started && !blockInputProcessed)
            {
                BlockInput = true;
                blockInputProcessed = true;
            }
            else if (context.canceled)
            {
                BlockInput = false;
                blockInputProcessed = false;
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
            if (context.started && !hotbarActionInputProcessed)
            {
                HotbarActionInput = true;
                hotbarActionInputProcessed = true;
            }
            else if (context.canceled)
            {
                HotbarActionInput = false;
                hotbarActionInputProcessed = false;
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
            if (context.started && !itemSwitchLeftInputProcessed)
            {
                ItemSwitchLeftInput = true;
                itemSwitchLeftInputProcessed = true;
            }
            else if (context.canceled)
            {
                ItemSwitchLeftInput = false;
                itemSwitchLeftInputProcessed = false;
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
            if (context.started && !itemSwitchRightInputProcessed)
            {
                ItemSwitchRightInput = true;
                itemSwitchRightInputProcessed = true;
            }
            else if (context.canceled)
            {
                ItemSwitchRightInput = false;
                itemSwitchRightInputProcessed = false;
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
    /// Handles spell input events from the Input System.
    /// Determines the spell type based on the binding index (e.g., first, second, third).
    /// </summary>
    public void OnUseSpellInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            // Get the index of the triggered binding
            var bindingIndex = context.action.GetBindingIndexForControl(context.control);

            if (context.started)
            {
                SpellCastInput = true;
            }
            else if (context.canceled)
            {
                // Only reset SpellCastInput if this is the key we're currently using
                if (bindingIndex == UseSpellHotbarNumber)
                {
                    SpellCastInput = false;
                }
                return;
            }

            // Always update the hotbar number when any spell key changes
            switch (bindingIndex)
            {
                case 0: // First spell binding
                    UseSpellHotbarNumber = 0;
                    break;
                case 1: // Second spell binding
                    UseSpellHotbarNumber = 1;
                    break;
                case 2: // Third spell binding
                    UseSpellHotbarNumber = 2;
                    break;
                default:
                    UseSpellHotbarNumber = 0; // Default to first spell type
                    break;
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
            if (context.started && !jumpInputProcessed)
            {
                JumpInput = true;
                JumpInputStop = false;
                jumpInputStartTime = Time.time;
                jumpInputProcessed = true;
            }
            else if (context.canceled)
            {
                JumpInputStop = true;
                jumpInputProcessed = false;
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
            if (context.started && !actionInputProcessed)
            {
                ActionInput = true;
                actionInputProcessed = true;
                actionInputInputStartTime = Time.time;
            }
            else if (context.canceled)
            {
                ActionInput = false;
                actionInputProcessed = false;
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

    /// <summary>
    /// Handles interact input events from the Input System
    /// </summary>
    public void OnPlayerMenuInput(InputAction.CallbackContext context)
    {
        if (!isDialogueActive)
        {
            if (context.started)
                GameEvents.Instance.PlayerMenuOpen();
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

    /// <summary>
    /// Consumes the spell cast input
    /// </summary>
    /// <remarks>
    /// Resets both SpellCastInput and UseSpellType to prevent unintended spell casting
    /// </remarks>
    public void UseSpellCastInput()
    {
        SpellCastInput = false;
        UseSpellHotbarNumber = 0;
    }

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

    private void OnDialogueTrigger(bool isDialogueActive) => this.isDialogueActive = isDialogueActive;
    #endregion
}