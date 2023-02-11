using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    public Vector2 RawMovementInput { get; private set; }

    public bool DisableInput = false;

    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }
    public bool AttackInput { get; private set; }
    public bool BlockInput { get; private set; }

    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float dashInputStartTime;

    private void Start()
    {
        playerInput =  GetComponent<PlayerInput>();
   
        try
        {
            GameEvents.Instance.onPauseTrigger += EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger += EnableDisablePlayerInput;
        }
        catch
        {
        }
    }

    private void OnDestroy()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger -= EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger -= EnableDisablePlayerInput;
        }
        catch
        {
        }
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                AttackInput = true;
            }

            if (context.canceled)
            {
                AttackInput = false;
            }
        }
    }
    public void OnBlockInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                BlockInput = true;
            }

            if (context.canceled)
            {
                BlockInput = false;
            }
        }
    }
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

            if (context.canceled)
            {
                JumpInputStop = true;
            }
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                DashInput = true;
                DashInputStop = false;
                dashInputStartTime = Time.time;
            }
            else if (context.canceled)
            {
                DashInputStop = true;
            }
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started)
            {
                GameEvents.Instance.InteractTrigger(true);
            }
            else if (context.canceled)
            {
                GameEvents.Instance.InteractTrigger(false);
            }
        }
    }

    public void UseJumpInput() => JumpInput = false;

    public void UseDashInput() => DashInput = false;

    public void UseAttackInput() => AttackInput = false;
    public void UseBlockInput() => BlockInput = false;

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckDashInputHoldTime()
    {
        if (Time.time >= dashInputStartTime + inputHoldTime)
        {
            DashInput = false;
        }
    }

    private void EnableDisablePlayerInput(bool disable)
    {
        DisableInput = disable;
        if(!disable) NormInputX = 0;
    }
}