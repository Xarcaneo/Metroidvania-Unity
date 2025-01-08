using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the completion of a climbing action.
/// This state plays the climbing finish animation and transitions to idle.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Playing the climbing completion animation
/// - Ensuring player remains stationary during the animation
/// - Transitioning to idle state when complete
/// 
/// The state automatically transitions to:
/// - IdleState: When the climbing finish animation is complete
/// </remarks>
public class PlayerFinishClimb : PlayerState
{
    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Used to control player movement during the climbing finish animation,
    /// ensuring the player remains stationary.
    /// </remarks>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerFinishClimb state
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerFinishClimb(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Ensure player remains stationary during animation
    /// 3. Check for animation completion
    /// 
    /// The state will transition to IdleState when:
    /// - The climbing finish animation has completed
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Keep player stationary during climbing finish
        Movement?.SetVelocityX(0f);

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
