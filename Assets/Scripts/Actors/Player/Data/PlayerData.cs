using UnityEngine;

/// <summary>
/// ScriptableObject that contains all configurable data for the player character.
/// This includes movement parameters, state timings, and collision settings.
/// </summary>
[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    #region Shared Parameters
    [Header("Shared")]
    [Tooltip("Height of the player's collider when standing")]
    public float standColliderHeight = 1.87f;
    #endregion

    #region Movement Parameters
    [Header("Move State")]
    [Tooltip("Base movement speed of the player")]
    public float movementVelocity = 10f;

    [Header("Grounded State")]
    [Tooltip("Physics material used when no friction is desired")]
    public PhysicsMaterial2D noFriction;
    [Tooltip("Physics material used when full friction is desired")]
    public PhysicsMaterial2D fullFriction;
    #endregion

    #region Jump Parameters
    [Header("Jump State")]
    [Tooltip("Initial velocity applied when jumping")]
    public float jumpVelocity = 15f;
    [Tooltip("Maximum number of jumps before needing to touch ground")]
    public int amountOfJumps = 1;

    [Header("Wall Jump State")]
    [Tooltip("Velocity applied during wall jump (X = horizontal, Y = vertical)")]
    public Vector2 wallJumpVelocity = new Vector2(25, 25);
    [Tooltip("Duration of the wall jump state")]
    public float wallJumpTime = 0.1f;
    [Tooltip("Angle of the wall jump (X = horizontal multiplier, Y = vertical multiplier)")]
    public Vector2 wallJumpAngle = new Vector2(1, 2);
    [Tooltip("Time required touching wall before allowing wall jump")]
    public float wallTouchTime = 0.1f;
    #endregion

    #region Wall Interaction Parameters
    [Header("Wall Slide")]
    [Tooltip("Width of the collider when sliding on left wall")]
    public float WallSlideColliderWidthLeft = 1.7f;
    [Tooltip("Width of the collider when sliding on right wall")]
    public float WallSlideColliderWidthRight = 1.5f;
    [Tooltip("Base width of the collider when not wall sliding")]
    public float WallSlideColliderWidthBase = 0.92f;

    [Header("Wall Slide State")]
    [Tooltip("Downward velocity when wall sliding")]
    public float wallSlideVelocity = 3f;

    [Header("Ledge/Ladder Climb State")]
    [Tooltip("Starting position offset for ledge/ladder climb")]
    public Vector2 startOffset;
    [Tooltip("Position offset where ledge/ladder climb ends")]
    public Vector2 stopOffset;
    #endregion

    #region Air Control Parameters
    [Header("In Air State")]
    [Tooltip("Time window for jumping after leaving ground")]
    public float coyoteTime = 0.2f;
    [Tooltip("Multiplier applied to jump height when button is released early")]
    public float variableJumpHeightMultiplier = 0.5f;
    #endregion

    #region Special Movement Parameters
    [Header("Dash State")]
    [Tooltip("Velocity applied during dash (X = horizontal, Y = vertical)")]
    public Vector2 dashVelocity = new Vector2(30, 0);
    [Tooltip("Angle of the dash (X = horizontal multiplier, Y = vertical multiplier)")]
    public Vector2 dashAngle = new Vector2(1, 0);
    [Tooltip("Time before dash can be used again")]
    public float dashCooldown = 1.5f;
    [Tooltip("Duration of the dash")]
    public float dashTime = 0.2f;

    [Header("Roll State")]
    [Tooltip("Speed of the roll movement")]
    public float rollSpeed = 10.0f;
    [Tooltip("Duration of the roll animation")]
    public float rollDuration = 0.7f;
    [Tooltip("Time before roll can be used again")]
    public float rollCooldown = 1.0f;
    [Tooltip("Height of the collider during roll")]
    public float rollColliderHeight = 0.6f;

    [Header("Crouch State")]
    [Tooltip("Height of the collider when crouching")]
    public float crouchColliderHeight = 0.6f;
    #endregion

    #region Combat Parameters
    [Header("Attack State")]
    [Tooltip("Movement speed during each attack in combo")]
    public float[] attackMovementSpeed;
    [Tooltip("Time window to continue combo")]
    public float breakComboTime = 1.0f;
    #endregion

    #region Ladder Parameters
    [Header("Ladder Climb State")]
    [Tooltip("Vertical climbing speed on ladders")]
    public float climbingVelocity = 5f;
    [Tooltip("Time before allowing another ladder climb")]
    public float ladderClimbCooldown = 0.2f;
    [Tooltip("Time before allowing another ladder jump")]
    public float ladderJumpCooldown = 1.0f;
    [Tooltip("Distance from top to finish climbing")]
    public float climbFinishThresholdUp = 1.0f;
    [Tooltip("Length of raycast to detect ladder top")]
    public float ladderTopRaycastLength = 2.0f;
    [Tooltip("Offset from ground when on ladder")]
    public float groundOffset = 1.0f;

    [Header("Prepare Ladder Climb State")]
    [Tooltip("Starting position offset when climbing from bottom")]
    public float startPosUp = 2f;
    [Tooltip("Starting position offset when climbing from top")]
    public float startPosDown = 2f;
    #endregion

    #region Impact Parameters
    [Header("GroundHit State")]
    [Tooltip("Minimum downward velocity to trigger ground hit state")]
    public float velocityToHit = -30f;
    #endregion
}