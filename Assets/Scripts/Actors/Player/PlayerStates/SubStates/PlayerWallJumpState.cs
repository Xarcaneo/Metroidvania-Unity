using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's wall jump ability.
/// This state manages the jumping movement when pushing off from walls.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing wall jump direction and velocity
/// - Handling jump input consumption
/// - Controlling jump count
/// - Managing character facing direction
/// 
/// The state can transition to:
/// - InAirState: When wall jump time expires
/// </remarks>
public class PlayerWallJumpState : PlayerAbilityState
{
    #region State Variables
    /// <summary>
    /// Direction of the wall jump (-1 for left, 1 for right)
    /// </summary>
    private int wallJumpDirection;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerWallJumpState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerWallJumpState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the wall jump state
    /// </summary>
    /// <remarks>
    /// Sets up the wall jump by:
    /// 1. Consuming jump input
    /// 2. Resetting and updating jump count
    /// 3. Applying wall jump velocity and angle
    /// 4. Updating character facing direction
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Consume jump input and manage jump count
        player.InputHandler.UseJumpInput();
        player.JumpState.ResetAmountOfJumpsLeft();
        
        // Apply wall jump velocity and direction
        Movement?.SetVelocity(playerData.wallJumpVelocity, playerData.wallJumpAngle, wallJumpDirection);
        player.JumpState.DecreaseAmountOfJumpsLeft();
        
        // Update facing direction
        Movement?.CheckIfShouldFlip(wallJumpDirection);
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check wall jump duration
    /// 2. Set ability completion flag
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if wall jump time has expired
        if (Time.time >= startTime + playerData.wallJumpTime)
        {
            isAbilityDone = true;
        }
    }

    /// <summary>
    /// Determines the direction of the wall jump based on wall contact
    /// </summary>
    /// <param name="isTouchingWall">Whether the player is touching a wall</param>
    /// <remarks>
    /// Sets wall jump direction:
    /// - Away from wall when touching wall
    /// - In current facing direction when not touching wall
    /// </remarks>
    public void DetermineWallJumpDirection(bool isTouchingWall)
    {
        if (isTouchingWall)
        {
            // Jump away from wall
            wallJumpDirection = -Movement.FacingDirection;
        }
        else
        {
            // Jump in current facing direction
            wallJumpDirection = Movement.FacingDirection;
        }
    }
}