using UnityEngine;

/// <summary>
/// Superstate that handles player abilities like attacking, blocking, and special moves.
/// This state manages the execution of abilities and handles transitions after their completion.
/// </summary>
/// <remarks>
/// This state serves as a base class for all ability-based states like Attack, Block, and Roll.
/// It provides common functionality for ability execution and state transitions including:
/// - Tracking ability completion status
/// - Monitoring environmental conditions during ability execution
/// - Managing transitions after ability completion
/// 
/// The state automatically transitions after ability completion to:
/// - IdleState: When grounded and not moving vertically
/// - WallSlideState: When touching a wall (disables wall jump coyote time)
/// - LedgeClimbState: When at a wall ledge and not near a platform
/// - InAirState: When in the air and no other conditions are met
/// 
/// Derived states should set isAbilityDone to true when their specific ability execution is complete.
/// This triggers the automatic state transition logic.
/// </remarks>
public class PlayerAbilityState : PlayerState
{
    #region State Variables
    /// <summary>
    /// Flag indicating if the current ability has finished executing
    /// </summary>
    /// <remarks>
    /// Set to false when entering the state and should be set to true by derived states
    /// when their specific ability execution is complete. This triggers the automatic
    /// state transition logic in LogicUpdate.
    /// </remarks>
    protected bool isAbilityDone;

    /// <summary>
    /// Flag indicating if the player is touching the ground
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used to determine if player should
    /// transition to IdleState after ability completion. Also affects which
    /// state to transition to when near walls or ledges.
    /// </remarks>
    protected bool isGrounded;

    /// <summary>
    /// Flag indicating if the player is touching a wall
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used to determine if player should
    /// transition to WallSlideState or LedgeClimbState after ability completion.
    /// </remarks>
    protected bool isTouchingWall;

    /// <summary>
    /// Flag indicating if the player is at a ledge
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used in combination with isTouchingWall
    /// to determine if player should transition to LedgeClimbState after ability completion.
    /// A ledge is detected when there's a wall below but not at the player's height.
    /// </remarks>
    protected bool isTouchingLedge;

    /// <summary>
    /// Flag indicating if the player is near a platform
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Prevents transition to LedgeClimbState
    /// when near platforms, as ledge climbing is not needed in these cases.
    /// </remarks>
    protected bool isNearPlatform;

    /// <summary>
    /// Flag indicating if the player is on a slope
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Can be used by derived states to
    /// modify ability behavior when on slopes.
    /// </remarks>
    protected bool isOnSlope;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to movement-related functionality like velocity control
    /// and current velocity values. Used to check vertical velocity when determining
    /// state transitions. Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to collision detection functionality for ground, walls,
    /// ledges, slopes, and platforms. These collision checks are crucial for
    /// determining state transitions after ability completion.
    /// Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the PlayerAbilityState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    /// <remarks>
    /// The constructor is called when creating new ability states. It passes the
    /// required dependencies to the base PlayerState class.
    /// </remarks>
    public PlayerAbilityState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }
    #endregion

    #region State Methods
    /// <summary>
    /// Performs state checks for collision detection
    /// </summary>
    /// <remarks>
    /// Called every frame to update collision flags. These flags determine:
    /// - Ground contact for idle transitions
    /// - Wall contact for wall-related transitions
    /// - Ledge detection for climb transitions
    /// - Platform proximity for ledge climb prevention
    /// - Slope detection for ability modifications
    /// 
    /// All these checks are crucial for determining the appropriate state
    /// to transition to after the ability is complete.
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingLedge = CollisionSenses.LedgeHorizontal;
            isOnSlope = CollisionSenses.SlopeCheck();
            isNearPlatform = CollisionSenses.Platform;
        }
    }

    /// <summary>
    /// Called when entering the state
    /// </summary>
    /// <remarks>
    /// Initializes the state by:
    /// 1. Calling base class Enter method
    /// 2. Setting isAbilityDone to false to indicate ability execution has started
    /// 
    /// Derived states should override this method to set up their specific
    /// ability initialization, but should always call base.Enter() first.
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Check if ability is complete (isAbilityDone)
    /// 3. Handle state transitions if ability is complete
    /// 
    /// The actual ability execution logic should be implemented in derived states.
    /// This base class only handles the transition logic after ability completion.
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAbilityDone)
        {
            HandleStateTransition();
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Handles state transitions after ability completion
    /// </summary>
    /// <remarks>
    /// Determines the appropriate state to transition to based on current conditions:
    /// 1. IdleState: When grounded and not moving vertically
    /// 2. WallSlideState: When touching a wall (disables wall jump coyote time)
    /// 3. LedgeClimbState: When at a wall ledge and not near a platform
    /// 4. InAirState: When in the air and no other conditions are met
    /// 
    /// The order of these checks is important as it determines transition priority.
    /// </remarks>
    private void HandleStateTransition()
    {
        if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
        {
            // Transition to idle if grounded and not moving up
            stateMachine.ChangeState(player.IdleState);
        }
        else if (isTouchingWall)
        {
            // Transition to wall slide if touching a wall
            player.WallSlideState.DisableWallJumpCoyoteTime();
            stateMachine.ChangeState(player.WallSlideState);
        }
        else if (isTouchingWall && !isTouchingLedge && !isGrounded && !isNearPlatform)
        {
            // Transition to ledge climb if at a wall ledge
            stateMachine.ChangeState(player.LedgeClimbState);
        }
        else
        {
            // Default to in-air state if no other conditions are met
            stateMachine.ChangeState(player.InAirState);
        }
    }
    #endregion
}