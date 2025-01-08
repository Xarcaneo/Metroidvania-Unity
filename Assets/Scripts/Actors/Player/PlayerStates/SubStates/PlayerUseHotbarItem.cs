using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's item usage from the hotbar.
/// This state manages the animation and physics during item usage.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing item use animation
/// - Handling slope physics during use
/// - Preventing movement during item use
/// 
/// The state can transition to:
/// - IdleState: When item use animation completes
/// </remarks>
public class PlayerUseHotbarItem : PlayerState
{
    #region State Variables
    /// <summary>
    /// Whether the player is currently on a slope
    /// </summary>
    protected bool isOnSlope;
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
    /// Initializes a new instance of the PlayerUseHotbarItem state
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerUseHotbarItem(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs necessary checks for the item use state
    /// </summary>
    /// <remarks>
    /// Checks if the player is on a slope to adjust physics behavior
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isOnSlope = CollisionSenses.SlopeCheck();
        }
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Stop horizontal movement
    /// 2. Manage slope physics
    /// 3. Check for state completion
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Prevent horizontal movement during item use
        Movement?.SetVelocityX(0f);

        // Adjust physics material based on slope
        if (isOnSlope)
        {
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
        }
        else
        {
            player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }

        // Check for state completion
        if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    /// <summary>
    /// Triggered by animation events to use the hotbar item
    /// </summary>
    /// <remarks>
    /// Calls the hotbar's UseItem method to execute item functionality
    /// </remarks>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        Menu.GameMenu.Instance.gameHotbar.UseItem();
    }
}
