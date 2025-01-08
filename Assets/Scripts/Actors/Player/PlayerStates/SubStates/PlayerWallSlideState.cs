using UnityEngine;

/// <summary>
/// State that handles the player's wall sliding behavior.
/// This state manages the sliding movement and transitions when touching walls.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing wall slide movement
/// - Adjusting collider width during slide
/// - Handling wall jump coyote time
/// - Controlling slide velocity
/// 
/// The state inherits from PlayerTouchingWallState and adds specific
/// wall sliding mechanics and transitions.
/// </remarks>
public class PlayerWallSlideState : PlayerTouchingWallState
{
    #region State Variables
    /// <summary>
    /// Whether the player can still perform a wall jump after leaving the wall
    /// </summary>
    private bool canWallJumpCoyoteTime = true;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerWallSlideState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerWallSlideState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the wall slide state
    /// </summary>
    /// <remarks>
    /// Adjusts the player's collider width based on the facing direction:
    /// - Right-facing: Uses WallSlideColliderWidthRight
    /// - Left-facing: Uses WallSlideColliderWidthLeft
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Adjust collider width based on facing direction
        if(Movement.FacingDirection == 1)
            player.SetColliderWidth(playerData.WallSlideColliderWidthRight);
        else
            player.SetColliderWidth(playerData.WallSlideColliderWidthLeft);
    }

    /// <summary>
    /// Called when exiting the wall slide state
    /// </summary>
    /// <remarks>
    /// Handles wall jump coyote time and resets collider width:
    /// 1. Starts wall jump coyote time if enabled
    /// 2. Resets the coyote time flag
    /// 3. Restores base collider width
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        // Start wall jump coyote time if enabled
        if (canWallJumpCoyoteTime) 
            player.InAirState.StartWallJumpCoyoteTime();

        // Reset flags and collider
        canWallJumpCoyoteTime = true;
        player.SetColliderWidth(playerData.WallSlideColliderWidthBase);
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Apply wall slide velocity
    /// 2. Control downward movement speed
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            // Apply wall slide velocity
            Movement?.SetVelocityY(-playerData.wallSlideVelocity);
        }
    }

    /// <summary>
    /// Disables the wall jump coyote time
    /// </summary>
    /// <returns>Always returns false to indicate coyote time is disabled</returns>
    /// <remarks>
    /// Used to prevent wall jump after certain conditions are met
    /// </remarks>
    public bool DisableWallJumpCoyoteTime() => canWallJumpCoyoteTime = false;
}
