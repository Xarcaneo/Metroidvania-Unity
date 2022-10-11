using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int amountOfJumps = 1;

    [Header("Wall Jump State")]
    public Vector2 wallJumpVelocity = new Vector2(25, 25);
    public float wallJumpTime = 0.2f;
    public Vector2 wallJumpAngle = new Vector2(1, 2);
    public float wallTouchTime = 0.1f;

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 3f;

    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;

    [Header("Dash State")]
    public Vector2 dashVelocity = new Vector2(30, 0);
    public Vector2 dashAngle = new Vector2(1, 0);
    public float dashCooldown = 1.5f;
    public float dashTime = 0.2f;
    public float dashEndYMultiplier = 0.2f;

    [Header("Crouch State")]
    public float standColliderHeight = 1.87f;
    public float crouchColliderHeight = 0.6f;

    [Header("Attack State")]
    public float[] attackMovementSpeed;
    public float breakComboTime = 1.0f;

    [Header("Interaction Offsets")]
    public float checkpointActivationOffset = 1.0f;
}