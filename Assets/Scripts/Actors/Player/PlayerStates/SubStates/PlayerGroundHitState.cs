using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's ground hit behavior.
/// This state is activated when the player hits the ground with high velocity.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing the impact animation when landing hard
/// - Stopping horizontal movement during impact
/// - Transitioning back to idle state after impact recovery
/// 
/// The state can transition to:
/// - IdleState: When the impact animation finishes
/// </remarks>
public class PlayerGroundHitState : PlayerState
{
    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerGroundHitState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerGroundHitState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Maintain zero horizontal velocity during impact
    /// 2. Check for transition to idle state when impact animation finishes
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
