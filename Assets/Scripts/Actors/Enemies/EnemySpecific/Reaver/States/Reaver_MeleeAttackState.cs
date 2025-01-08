using UnityEngine;

/// <summary>
/// Handles the melee attack behavior for the Reaver enemy.
/// Manages attack execution, blocking detection, and state transitions.
/// </summary>
public class Reaver_MeleeAttackState : MeleeAttackState
{
    private readonly Reaver enemy;

    // Cached components
    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    private bool attackBlockedByDefender = false;

    /// <summary>
    /// Initializes a new instance of the Reaver_MeleeAttackState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the melee attack</param>
    /// <param name="enemy">Reference to the Reaver enemy instance</param>
    public Reaver_MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_MeleeAttack stateData, Reaver enemy) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// Called when entering the melee attack state.
    /// Sets up attack properties and ensures proper facing direction.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        SetupAttackState();
    }

    /// <summary>
    /// Called when exiting the melee attack state.
    /// Cleans up event subscriptions and resets state.
    /// </summary>
    public override void Exit()
    {
        DamageReceiver.OnAttackBlockedByDefender -= OnAttackBlockedByDefender;
        attackBlockedByDefender = false;
        base.Exit();
    }

    /// <summary>
    /// Updates the logical state of the melee attack behavior.
    /// Handles transitions to cooldown state based on animation or block status.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if ((isAnimationFinished && !isExitingState) || attackBlockedByDefender)
        {
            stateMachine.ChangeState(enemy.attackCooldownState);
        }
    }

    /// <summary>
    /// Sets up the initial state for the attack, including event handlers and positioning.
    /// </summary>
    private void SetupAttackState()
    {
        DamageReceiver.OnAttackBlockedByDefender += OnAttackBlockedByDefender;
        
        if (Movement.FacingDirection != EnemyDamageHitBox.entityToRight)
        {
            Movement.Flip();
        }

        Movement?.SetVelocityX(0f);
        m_damageData.CanBlock = true;
    }

    /// <summary>
    /// Callback for when the attack is blocked by a defender.
    /// </summary>
    private void OnAttackBlockedByDefender() => attackBlockedByDefender = true;
}