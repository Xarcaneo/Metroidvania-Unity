using UnityEngine;

/// <summary>
/// Represents the Reaver enemy type, a melee-focused enemy that patrols and chases the player.
/// </summary>
public class Reaver : Enemy
{
    #region States
    public Reaver_IdleState idleState { get; private set; }
    public Reaver_PatrolState patrolState { get; private set; }
    public Reaver_DeathState deathState { get; private set; }
    public Reaver_ChaseState chaseState { get; private set; }
    public Reaver_WaitingState waitingState { get; private set; }
    public Reaver_MeleeAttackState meleeAttackState { get; private set; }
    public Reaver_AttackCooldownState attackCooldownState { get; private set; }
    #endregion

    #region State Configuration Data
    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_ChaseState chaseData;
    [SerializeField]
    private D_MeleeAttack meleeAttackData;
    [SerializeField]
    private D_AttackCooldownState attackCooldownData;
    #endregion

    /// <summary>
    /// Initializes all states during Awake.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        InitializeStates();
    }

    /// <summary>
    /// Initializes and starts the enemy with patrol state.
    /// </summary>
    public override void Start()
    {
        base.Start();
        StateMachine.Initialize(patrolState);
    }

    /// <summary>
    /// Returns the death state for this enemy.
    /// </summary>
    /// <returns>The death state instance.</returns>
    public override State GetDeathState()
    {
        return deathState;
    }

    /// <summary>
    /// Initializes all state instances with their respective configuration data.
    /// </summary>
    private void InitializeStates()
    {
        idleState = new Reaver_IdleState(this, StateMachine, "idle", idleStateData);
        patrolState = new Reaver_PatrolState(this, StateMachine, "move", moveStateData);
        deathState = new Reaver_DeathState(this, StateMachine, "death");
        chaseState = new Reaver_ChaseState(this, StateMachine, "chase", chaseData);
        waitingState = new Reaver_WaitingState(this, StateMachine, "waiting");
        meleeAttackState = new Reaver_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackData);
        attackCooldownState = new Reaver_AttackCooldownState(this, StateMachine, "attackCooldown", attackCooldownData);
    }
}
