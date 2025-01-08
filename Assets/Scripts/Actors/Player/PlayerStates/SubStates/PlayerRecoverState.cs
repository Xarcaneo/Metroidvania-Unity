using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's recovery after being hit or knocked back.
/// This state manages the transition from a vulnerable state back to normal gameplay.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing recovery animation
/// - Checking for ground contact
/// - Preventing movement during recovery
/// - Transitioning to landing state when ready
/// 
/// The state can transition to:
/// - LandState: When recovery animation completes and player is grounded
/// </remarks>
public class PlayerRecoverState : PlayerState
{
    #region State Variables
    /// <summary>
    /// Whether the player is touching the ground
    /// </summary>
    private bool isGrounded;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerRecoverState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerRecoverState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs necessary checks for the recover state
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
    /// 1. Prevent horizontal movement
    /// 2. Check for recovery completion
    /// 3. Transition to land state when conditions are met
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Prevent horizontal movement during recovery
        Movement?.SetVelocityX(0f);

        // Transition to land state when recovery is complete and grounded
        if (isAnimationFinished && !isExitingState && isGrounded)
        {
            stateMachine.ChangeState(player.LandState);
        }
    }
}
