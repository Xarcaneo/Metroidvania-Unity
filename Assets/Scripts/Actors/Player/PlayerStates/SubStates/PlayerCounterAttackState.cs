using UnityEngine;

/// <summary>
/// State that handles the player's counter-attack ability.
/// This is a special attack state that follows a successful block.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Executing a counter-attack after a successful block
/// - Dealing damage to enemies
/// - Applying knockback effects
/// - Managing attack animations
/// 
/// The state automatically transitions to:
/// - IdleState: When the counter-attack animation is finished
/// </remarks>
public class PlayerCounterAttackState : PlayerState
{
    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Used to control player movement during counter-attack and
    /// get facing direction for knockback effects.
    /// </remarks>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the DamageHitBox component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Handles collision detection and damage application to enemies
    /// during the counter-attack.
    /// </remarks>
    private DamageHitBox DamageHitBox { get => damageHitBox ?? core.GetCoreComponent(ref damageHitBox); }
    private DamageHitBox damageHitBox;

    /// <summary>
    /// Reference to the Stats component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to player's attack stats for damage calculation.
    /// </remarks>
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;
    #endregion

    /// <summary>
    /// Data structure containing damage information for the counter-attack
    /// </summary>
    /// <remarks>
    /// Stores information about:
    /// - Damage source (the player)
    /// - Damage amount (from player's attack stat)
    /// - Any additional effects
    /// </remarks>
    private IDamageable.DamageData m_damageData;

    /// <summary>
    /// Initializes a new instance of the PlayerCounterAttackState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerCounterAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the counter-attack state
    /// </summary>
    /// <remarks>
    /// Sets up the counter-attack by:
    /// 1. Calling base class Enter method
    /// 2. Setting up damage data with player's attack stat
    /// 3. Consuming the attack input to prevent immediate re-trigger
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Initialize damage data with player's attack stat
        m_damageData.SetData(player, Stats.GetAttack());
        player.InputHandler.UseAttackInput();
    }

    /// <summary>
    /// Called when exiting the counter-attack state
    /// </summary>
    /// <remarks>
    /// Cleans up the counter-attack state by:
    /// 1. Calling base class Exit method
    /// 2. Stopping any horizontal movement
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        // Stop horizontal movement when exiting
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Ensure player remains stationary during counter-attack
    /// 3. Check for state transition conditions
    /// 
    /// The state will transition to IdleState when:
    /// - The player is not currently exiting the state AND
    /// - The counter-attack animation has finished
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during counter-attack
        Movement?.SetVelocityX(0f);

        if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    /// <summary>
    /// Called by animation events during specific attack frames
    /// </summary>
    /// <remarks>
    /// Handles the actual damage application during the counter-attack:
    /// 1. Calls base class AnimationActionTrigger
    /// 2. Applies melee damage to intersecting enemies
    /// 3. Applies knockback effect in player's facing direction
    /// </remarks>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        // Apply damage and knockback to enemies in weapon hitbox
        DamageHitBox?.MeleeAttack(m_damageData);
        DamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);
    }
}
