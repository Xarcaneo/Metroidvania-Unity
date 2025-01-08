using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Skeleton enemy type that inherits from the base Enemy class.
/// This class manages different states of the skeleton's behavior including idle, patrol, chase, and attack states.
/// </summary>
public class Skeleton : Enemy
{
    #region States
    public Skeleton_IdleState idleState { get; private set; }
    public Skeleton_PatrolState patrolState { get; private set; }
    public Skeleton_ChaseState chaseState { get; private set; }
    public Skeleton_WaitingState waitingState { get; private set; }
    public Skeleton_DeathState deathState { get; private set; }
    public Skeleton_MeleeAttackState meleeAttackState { get; private set; }
    #endregion

    #region State Data
    [SerializeField, Tooltip("Configuration data for the idle state")]
    private D_IdleState idleStateData;
    [SerializeField, Tooltip("Configuration data for movement states")]
    private D_MoveState moveStateData;
    [SerializeField, Tooltip("Configuration data for the chase state")]
    private D_ChaseState chaseStateData;
    #endregion

    /// <summary>
    /// Initializes the skeleton's states on awake.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        InitializeStates();
    }

    /// <summary>
    /// Returns the death state for this skeleton.
    /// </summary>
    /// <returns>The skeleton's death state.</returns>
    public override State GetDeathState()
    {
        return deathState;
    }

    /// <summary>
    /// Initializes the skeleton's starting state.
    /// </summary>
    public override void Start()
    {
        base.Start();
        StateMachine.Initialize(patrolState);
    }

    /// <summary>
    /// Creates instances of all states with their respective configurations.
    /// </summary>
    private void InitializeStates()
    {
        idleState = new Skeleton_IdleState(this, StateMachine, "idle", idleStateData);
        patrolState = new Skeleton_PatrolState(this, StateMachine, "move", moveStateData);
        chaseState = new Skeleton_ChaseState(this, StateMachine, "move", chaseStateData);
        waitingState = new Skeleton_WaitingState(this, StateMachine, "idle");
        deathState = new Skeleton_DeathState(this, StateMachine, "death");
        meleeAttackState = new Skeleton_MeleeAttackState(this, StateMachine, "meleeAttack");
    }
}
