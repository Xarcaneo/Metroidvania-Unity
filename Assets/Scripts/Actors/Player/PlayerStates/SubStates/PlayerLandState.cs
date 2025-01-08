using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's landing behavior.
/// This state is activated when the player touches ground after being airborne.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing landing animation
/// - Handling transitions based on player input
/// - Ensuring smooth landing-to-movement transitions
/// 
/// The state can transition to:
/// - MoveState: When horizontal input is detected during landing
/// - IdleState: When landing animation completes with no input
/// </remarks>
public class PlayerLandState : PlayerGroundedState
{
    /// <summary>
    /// Initializes a new instance of the PlayerLandState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerLandState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check for horizontal input during landing
    /// 2. Transition to move state if input detected
    /// 3. Transition to idle state when landing completes
    /// 
    /// Priority is given to movement input, allowing for smooth landing-to-movement transitions
    /// even before the landing animation completes.
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            // Prioritize movement input over landing completion
            if (xInput != 0)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            // If no movement input and landing animation finished, go to idle
            else if (isAnimationFinished)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
