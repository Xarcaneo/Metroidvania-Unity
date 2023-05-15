using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Shared")]
    public float standColliderHeight = 1.87f;

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

    [Header("Wall Slide")]
    public float WallSlideColliderWidthLeft = 1.7f;
    public float WallSlideColliderWidthRight = 1.5f;
    public float WallSlideColliderWidthBase = 0.92f;

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

    [Header("Roll State")]
    public float rollSpeed = 10.0f;
    public float rollDuration = 0.7f;
    public float rollCooldown = 1.0f;
    public float rollColliderHeight = 0.6f;

    [Header("Crouch State")]
    public float crouchColliderHeight = 0.6f;

    [Header("Attack State")]
    public float[] attackMovementSpeed;
    public float breakComboTime = 1.0f;

    [Header("Ladder Climb State")]
    public float climbingVelocity = 5f;
    public float ladderJumpCooldown = 1.0f;
}