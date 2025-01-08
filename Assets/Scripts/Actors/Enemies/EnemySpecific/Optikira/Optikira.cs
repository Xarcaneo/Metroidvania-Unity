using UnityEngine;

/// <summary>
/// Represents the Optikira enemy type, a creature that can patrol, dash, and perform ranged attacks.
/// </summary>
public class Optikira : Enemy
{
    #region States
    public Optikira_IdleState idleState { get; private set; }
    public Optikira_DeathState deathState { get; private set; }
    public Optikira_RangedAttackState rangedAttackState { get; private set; }
    public Optikira_AttackCooldownState attackCooldownState { get; private set; }
    public Optikira_DashState dashState { get; private set; }
    public Optikira_PatrolState patrolState { get; private set; }
    #endregion

    #region State Configuration Data
    [Header("State Configuration")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_RangedAttackState rangedAttackData;
    [SerializeField] private D_AttackCooldownState attackCooldownData;
    [SerializeField] private D_DashState dashData;
    [SerializeField] private D_MoveState moveStateData;
    #endregion

    #region References
    [Header("Transform References")]
    [SerializeField] private Transform attackPosition;
    #endregion

    /// <summary>
    /// Initializes all Optikira's states during Awake.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        InitializeStates();
    }

    /// <summary>
    /// Starts the Optikira with its initial patrol state.
    /// </summary>
    public override void Start()
    {
        base.Start();
        StateMachine.Initialize(patrolState);
    }

    /// <summary>
    /// Returns the death state for this enemy.
    /// </summary>
    /// <returns>The Optikira's death state</returns>
    public override State GetDeathState()
    {
        return deathState;
    }

    /// <summary>
    /// Creates instances of all states with their respective configurations.
    /// </summary>
    private void InitializeStates()
    {
        idleState = new Optikira_IdleState(this, StateMachine, "idle", idleStateData, this);
        deathState = new Optikira_DeathState(this, StateMachine, "death", this);
        rangedAttackState = new Optikira_RangedAttackState(this, StateMachine, "attack", rangedAttackData, attackPosition);
        attackCooldownState = new Optikira_AttackCooldownState(this, StateMachine, "attackCooldown", attackCooldownData);
        dashState = new Optikira_DashState(this, StateMachine, "dash", dashData);
        patrolState = new Optikira_PatrolState(this, StateMachine, "move", moveStateData);
    }
}
