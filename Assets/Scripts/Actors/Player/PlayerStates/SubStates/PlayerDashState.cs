using UnityEngine;

/// <summary>
/// State that handles the player's dash ability.
/// This state manages quick directional movement with specific timing and cooldown.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing dash movement and velocity
/// - Handling dash cooldown timing
/// - Detecting collision-based dash interruptions
/// - Transitioning out when dash completes
/// 
/// The dash ability ends when:
/// - Dash duration expires
/// - Player hits a wall
/// - Player hits a slope while grounded
/// - Player touches a ledge
/// </remarks>
public class PlayerDashState : PlayerAbilityState
{
    #region State Variables
    /// <summary>
    /// Time when the last dash was performed
    /// </summary>
    /// <remarks>
    /// Used to enforce dash cooldown between uses
    /// </remarks>
    private float lastDashTime;

    /// <summary>
    /// Direction vector for the current dash
    /// </summary>
    /// <remarks>
    /// Currently only supports horizontal dashing (right or left)
    /// based on player's facing direction
    /// </remarks>
    private Vector2 dashDirection;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerDashState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerDashState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the dash state
    /// </summary>
    /// <remarks>
    /// Sets up the dash by:
    /// 1. Calling base class Enter method
    /// 2. Consuming the dash input
    /// 3. Setting up dash direction based on facing direction
    /// 4. Recording start time
    /// 5. Checking if player should flip based on dash direction
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Consume dash input to prevent immediate re-trigger
        player.InputHandler.UseDashInput();

        // Set up dash direction based on facing direction
        dashDirection = Vector2.right * Movement.FacingDirection;

        // Record dash start time and check if player should flip
        startTime = Time.time;
        Movement?.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Check for dash completion conditions
    /// 3. Apply or stop dash velocity
    /// 
    /// The dash will complete when:
    /// - Dash duration has expired
    /// - Player hits a wall
    /// - Player hits a slope while grounded
    /// - Player touches a ledge
    /// 
    /// When completed:
    /// - Sets isAbilityDone flag
    /// - Records last dash time for cooldown
    /// - Stops player movement
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (Time.time >= startTime + playerData.dashTime || 
                isTouchingWall || 
                isOnSlope && isGrounded || 
                isTouchingLedge)
            {
                // Complete dash and stop movement
                isAbilityDone = true;
                lastDashTime = Time.time;
                Movement?.SetVelocityZero();
            }
            else
            {
                // Apply dash velocity in the dash direction
                Movement?.SetVelocity(playerData.dashVelocity, playerData.dashAngle, Movement.FacingDirection);
            }
        }
    }

    /// <summary>
    /// Checks if the dash ability is currently available
    /// </summary>
    /// <remarks>
    /// Dash becomes available when the cooldown period has expired
    /// since the last dash was performed.
    /// </remarks>
    /// <returns>True if enough time has passed since the last dash, false otherwise</returns>
    public bool CheckIfCanDash()
    {
        return Time.time >= lastDashTime + playerData.dashCooldown;
    }
}