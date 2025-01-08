using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's initial spawn behavior.
/// This state manages the transition from spawning to normal gameplay.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing spawn animation
/// - Checking for ground contact
/// - Transitioning to idle when grounded
/// 
/// The state can transition to:
/// - IdleState: When player touches ground and vertical velocity is near zero
/// </remarks>
public class PlayerSpawnedState : PlayerState
{
    #region State Variables
    /// <summary>
    /// Whether the player is touching the ground
    /// </summary>
    private bool isGrounded;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerSpawnedState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerSpawnedState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs necessary checks for the spawn state
    /// </summary>
    /// <remarks>
    /// Checks if the player has made contact with the ground
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
        }
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check for ground contact
    /// 2. Verify vertical velocity is near zero
    /// 3. Transition to idle state when conditions are met
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Transition to idle when grounded and nearly stopped
        if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
