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
    
    /// <summary>
    /// Reference to the DamageReceiver component, lazily loaded
    /// </summary>
    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    #endregion
    
    #region State Variables
    /// <summary>
    /// Indicates whether a parryable attack was blocked
    /// </summary>
    private bool canParry = false;
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
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
        // canParry is now set by PrepareBlockState via SetCanParry
    }

    /// <summary>
    /// Called when exiting the block state
    /// </summary>
    /// <remarks>
    /// Cleans up the blocking state by:
    /// 1. Calling base class Exit method
    /// 2. Resetting state variables
    /// </remarks>
    public override void Exit()
    {
        base.Exit();
        
        // Reset state variables
        canParry = false;
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
    /// The state will transition to:
    /// - CounterAttackState: When the block animation has finished AND a parryable attack was blocked
    /// - IdleState: When the block animation has finished but no parryable attack was blocked
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during block
        Movement?.SetVelocityX(0f);

        if (!isExitingState && isAnimationFinished)
        {
            if (canParry)
            {
                // Transition to counter attack if a parryable attack was blocked
                stateMachine.ChangeState(player.CounterAttackState);
            }
            else
            {
                // Transition to idle if no parryable attack was blocked
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
    
    /// <summary>
    /// Sets whether the player can parry after this block
    /// </summary>
    /// <param name="canParry">Whether the player can parry</param>
    public void SetCanParry(bool canParry)
    {
        this.canParry = canParry;
    }
}
