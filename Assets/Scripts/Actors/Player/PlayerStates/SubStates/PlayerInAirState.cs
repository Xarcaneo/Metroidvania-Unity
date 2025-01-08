using UnityEngine;

/// <summary>
/// State that handles the player's in-air behavior.
/// This state manages jumping, falling, and various mid-air transitions.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing player movement while airborne
/// - Handling various mid-air transitions (wall slide, ledge grab, etc.)
/// - Processing jump inputs and modifiers
/// - Managing coyote time mechanics
/// - Handling ground impact detection
/// 
/// The state can transition to:
/// - GroundHitState: When landing with high velocity
/// - JumpAttackState: When attack input is detected
/// - LandState: When touching ground with low velocity
/// - LedgeClimbState: When touching a wall near a ledge
/// - JumpState: When jump is available and requested
/// - WallSlideState: When touching a wall while moving towards it
/// - LadderClimbState: When touching a ladder and moving up
/// - DashState: When dash is available and requested
/// </remarks>
public class PlayerInAirState : PlayerState
{
    #region Input Variables
    /// <summary>
    /// Horizontal input value (-1 for left, 0 for neutral, 1 for right)
    /// </summary>
    private int xInput;

    /// <summary>
    /// Vertical input value (-1 for down, 0 for neutral, 1 for up)
    /// </summary>
    private int yInput;

    /// <summary>
    /// Flag indicating if jump input is active
    /// </summary>
    private bool jumpInput;

    /// <summary>
    /// Flag indicating if jump input has been released
    /// </summary>
    private bool jumpInputStop;

    /// <summary>
    /// Flag indicating if action input (dash) is active
    /// </summary>
    private bool actionInput;

    /// <summary>
    /// Flag indicating if attack input is active
    /// </summary>
    private bool attackInput;
    #endregion

    #region Check Variables
    /// <summary>
    /// Flag indicating if player is touching ground
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// Flag indicating if player is touching a wall
    /// </summary>
    private bool isTouchingWall;

    /// <summary>
    /// Flag indicating if player is near a platform
    /// </summary>
    private bool isNearPlatform;

    /// <summary>
    /// Flag indicating if player is touching a ladder
    /// </summary>
    private bool isTouchingLadder;
    #endregion

    #region State Variables
    /// <summary>
    /// Flag indicating if coyote time is active
    /// </summary>
    /// <remarks>
    /// Coyote time allows the player to jump shortly after walking off a ledge
    /// </remarks>
    private bool coyoteTime;

    /// <summary>
    /// Flag indicating if wall jump coyote time is active
    /// </summary>
    /// <remarks>
    /// Wall jump coyote time allows the player to wall jump shortly after leaving a wall
    /// </remarks>
    protected bool wallJumpCoyoteTime;

    /// <summary>
    /// Flag indicating if player is currently in jumping state
    /// </summary>
    private bool isJumping;

    /// <summary>
    /// Flag indicating if player is touching a ledge
    /// </summary>
    private bool isTouchingLedge;

    /// <summary>
    /// Maximum downward velocity reached during fall
    /// </summary>
    /// <remarks>
    /// Used to determine if player should enter ground hit state on landing
    /// </remarks>
    private float maxReachedVelocityY;

    /// <summary>
    /// Time when wall jump coyote time started
    /// </summary>
    private float startWallJumpCoyoteTime;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerInAirState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerInAirState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    /// <remarks>
    /// Updates collision flags and handles ledge detection:
    /// 1. Checks ground contact
    /// 2. Checks wall contact
    /// 3. Checks platform proximity
    /// 4. Checks ledge contact
    /// 5. Checks ladder contact
    /// 6. Updates ledge climb position if needed
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isNearPlatform = CollisionSenses.Platform;
            isTouchingLedge = CollisionSenses.LedgeHorizontal;
            isTouchingLadder = collisionSenses.Ladder;
        }

        // Update ledge climb position if touching wall but not ledge
        if (isTouchingWall && !isTouchingLedge)
        {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }
    }

    /// <summary>
    /// Called when exiting the in-air state
    /// </summary>
    /// <remarks>
    /// Cleans up the in-air state by:
    /// 1. Calling base class Exit method
    /// 2. Resetting wall contact flag
    /// 3. Resetting maximum fall velocity
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        isTouchingWall = false;
        maxReachedVelocityY = 0.0f;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Update coyote time mechanics
    /// 2. Track maximum fall velocity
    /// 3. Process player inputs
    /// 4. Apply jump modifiers
    /// 5. Check for state transitions
    /// 6. Update movement and animations
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckCoyoteTime();
        CheckWallJumpCoyoteTime();

        // Track maximum fall velocity
        if (Movement?.CurrentVelocity.y < maxReachedVelocityY)
            maxReachedVelocityY = (float)(Movement?.CurrentVelocity.y);

        // Get player inputs
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        actionInput = player.InputHandler.ActionInput;
        attackInput = player.InputHandler.AttackInput;

        CheckJumpMultiplier();

        // Handle state transitions
        if (isGrounded && maxReachedVelocityY <= playerData.velocityToHit)
        {
            stateMachine.ChangeState(player.GroundHitState);
        }
        else if (attackInput)
        {
            stateMachine.ChangeState(player.JumpAttackState);
        }
        else if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (isTouchingWall && !isTouchingLedge && !isGrounded && !isNearPlatform)
        {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
        else if (jumpInput && player.JumpState.CanJump() && Movement?.CurrentVelocity.y == 0.00f)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isTouchingWall && xInput == Movement?.FacingDirection && Movement?.CurrentVelocity.y <= 0)
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
        else if (yInput == 1 && isTouchingLadder && Movement?.CurrentVelocity.y <= 0)
        {
            stateMachine.ChangeState(player.LadderClimbState);
        }
        else if (actionInput && player.DashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
        else
        {
            // Update movement and animations
            Movement?.CheckIfShouldFlip(xInput);
            Movement?.SetVelocityX(playerData.movementVelocity * xInput);

            player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));
        }
    }

    /// <summary>
    /// Applies variable jump height mechanics
    /// </summary>
    /// <remarks>
    /// Modifies jump velocity based on:
    /// - Early jump button release
    /// - Reaching apex of jump
    /// </remarks>
    private void CheckJumpMultiplier()
    {
        if (isJumping)
        {
            if (jumpInputStop)
            {
                // Reduce jump height if button released early
                Movement?.SetVelocityY(Movement.CurrentVelocity.y * playerData.variableJumpHeightMultiplier);
                isJumping = false;
            }
            else if (Movement?.CurrentVelocity.y <= 0f)
            {
                // End jumping state at apex
                isJumping = false;
            }
        }
    }

    /// <summary>
    /// Manages coyote time mechanics
    /// </summary>
    /// <remarks>
    /// Coyote time allows the player to jump shortly after walking off a ledge.
    /// When coyote time expires, reduces available jumps.
    /// </remarks>
    private void CheckCoyoteTime()
    {   
        if (coyoteTime && Time.time > startTime + playerData.coyoteTime)
        {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    /// <summary>
    /// Manages wall jump coyote time mechanics
    /// </summary>
    /// <remarks>
    /// Wall jump coyote time allows the player to perform a wall jump
    /// shortly after leaving a wall.
    /// </remarks>
    private void CheckWallJumpCoyoteTime()
    {
        if (wallJumpCoyoteTime && Time.time > startWallJumpCoyoteTime + playerData.coyoteTime)
        {
            wallJumpCoyoteTime = false;
        }
    }

    /// <summary>
    /// Starts the coyote time window
    /// </summary>
    public void StartCoyoteTime() => coyoteTime = true;

    /// <summary>
    /// Starts the wall jump coyote time window
    /// </summary>
    public void StartWallJumpCoyoteTime()
    {
        wallJumpCoyoteTime = true;
        startWallJumpCoyoteTime = Time.time;
    }

    /// <summary>
    /// Stops the wall jump coyote time window
    /// </summary>
    public void StopWallJumpCoyoteTime() => wallJumpCoyoteTime = false;

    /// <summary>
    /// Sets the jumping flag to true
    /// </summary>
    public void SetIsJumping() => isJumping = true;
}