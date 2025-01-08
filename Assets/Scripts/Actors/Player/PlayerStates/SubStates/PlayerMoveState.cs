using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's movement while on the ground.
/// This state manages horizontal movement, slope handling, and transitions to other ground states.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing horizontal movement velocity
/// - Handling movement on slopes
/// - Checking for state transitions based on input
/// 
/// The state can transition to:
/// - IdleState: When no horizontal input is detected
/// - CrouchIdleState: When down input is detected
/// </remarks>
public class PlayerMoveState : PlayerGroundedState
{
    /// <summary>
    /// Initializes a new instance of the PlayerMoveState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerMoveState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs necessary checks for the move state
    /// </summary>
    /// <remarks>
    /// Inherits ground checks from base class
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();
    }

    /// <summary>
    /// Called when entering the move state
    /// </summary>
    /// <remarks>
    /// Inherits base enter behavior
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Called when exiting the move state
    /// </summary>
    /// <remarks>
    /// Inherits base exit behavior
    /// </remarks>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check and apply character facing direction
    /// 2. Apply movement velocity based on slope conditions
    /// 3. Check for transitions to other states based on input
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check and update facing direction based on input
        Movement?.CheckIfShouldFlip(xInput);

        if (!isExitingState)
        {
            // Apply movement velocity based on slope conditions
            if (!isOnSlope)
                Movement?.SetVelocityX(playerData.movementVelocity * xInput);
            else if (isOnSlope && !isTouchingWall)
                Movement?.SetVelocityXOnSlope(playerData.movementVelocity * -xInput);

            // Check for state transitions based on input
            if (xInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (yInput == -1)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
        }
    }
}