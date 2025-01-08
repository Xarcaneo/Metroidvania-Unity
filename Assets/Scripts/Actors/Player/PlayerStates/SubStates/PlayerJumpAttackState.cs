using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's aerial attack behavior.
/// This state is activated when the player attacks while in the air.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing aerial attack animations
/// - Handling damage and knockback application
/// - Transitioning back to in-air state after attack
/// 
/// The state can transition to:
/// - InAirState: When the attack animation finishes
/// </remarks>
public class PlayerJumpAttackState : PlayerInAirState 
{
    #region Input Variables
    /// <summary>
    /// Horizontal input value (-1 for left, 0 for neutral, 1 for right)
    /// </summary>
    private int xInput;
    #endregion

    #region Combat Variables
    /// <summary>
    /// Data structure containing damage and knockback information
    /// </summary>
    private IDamageable.DamageData m_damageData;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the DamageHitBox component, lazily loaded
    /// </summary>
    private DamageHitBox DamageHitBox { get => damageHitBox ?? core.GetCoreComponent(ref damageHitBox); }
    private DamageHitBox damageHitBox;

    /// <summary>
    /// Reference to the Stats component, lazily loaded
    /// </summary>
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerJumpAttackState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerJumpAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the jump attack state
    /// </summary>
    /// <remarks>
    /// Sets up the attack by:
    /// 1. Initializing damage data with player stats
    /// 2. Consuming the attack input
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        m_damageData.SetData(player, Stats.GetAttack());
        player.InputHandler.UseAttackInput();
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check for attack animation completion
    /// 2. Transition to in-air state when attack finishes
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState && isAnimationFinished) 
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    /// <summary>
    /// Triggered by animation events to apply damage and knockback
    /// </summary>
    /// <remarks>
    /// When triggered:
    /// 1. Checks for damageable entities within weapon hitbox
    /// 2. Applies damage to intersecting entities
    /// 3. Applies knockback in the facing direction
    /// </remarks>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        // Apply damage and knockback to entities in weapon hitbox
        DamageHitBox?.MeleeAttack(m_damageData);
        DamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);
    }
}
