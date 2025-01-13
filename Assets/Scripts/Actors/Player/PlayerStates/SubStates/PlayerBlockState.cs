using UnityEngine;

/// <summary>
/// State that handles the player's blocking behavior.
/// This state allows the player to defend against incoming attacks and potentially counter-attack.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing the player's block stance
/// - Preventing movement during block
/// - Transitioning to counter-attack when appropriate
/// 
/// The state automatically transitions to:
/// - CounterAttackState: When the block animation is finished
/// </remarks>
public class PlayerBlockState : PlayerState
{
    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Used to control player movement during blocking,
    /// ensuring the player remains stationary while in block stance.
    /// </remarks>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private bool isOnSlope;
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerBlockState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerBlockState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the block state
    /// </summary>
    /// <remarks>
    /// Sets up the blocking state by:
    /// 1. Calling base class Enter method
    /// 2. Any additional block-specific setup can be added here
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
        
        // Use full friction on slopes to prevent sliding
        player.RigidBody2D.sharedMaterial = playerData.fullFriction;
    }

    /// <summary>
    /// Called when exiting the block state
    /// </summary>
    /// <remarks>
    /// Cleans up the blocking state by:
    /// 1. Calling base class Exit method
    /// 2. Any additional block-specific cleanup can be added here
    /// </remarks>
    public override void Exit()
    {
        base.Exit();
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isOnSlope = CollisionSenses?.SlopeCheck() ?? false;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Ensure player remains stationary during block
    /// 3. Check for state transition conditions
    /// 
    /// The state will transition to CounterAttackState when:
    /// - The player is not currently exiting the state AND
    /// - The block animation has finished
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during block
        if (!isOnSlope)
        {
            Movement?.SetVelocityX(0f);
        }
        else
        {
            Movement?.SetVelocityXOnSlope(0f);
            Movement?.SetVelocityY(0f);
        }

        if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.CounterAttackState);
        }
    }
}
