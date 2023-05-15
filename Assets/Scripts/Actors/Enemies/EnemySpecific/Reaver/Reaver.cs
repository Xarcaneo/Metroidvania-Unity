 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver : Enemy
{
    public Reaver_IdleState idleState { get; private set; }
    public Reaver_PatrolState patrolState { get; private set; }
    public Reaver_DeathState deathState { get; private set; }
    public Reaver_ChaseState chasteState { get; private set; }
    public Reaver_WaitingState waitingState { get; private set; }
    public Reaver_MeleeAttackState meleeAttackState { get; private set; }
    public Reaver_AttackCooldownState attackCooldownState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_ChaseState chaseData;
    [SerializeField]
    private D_MeleeAttack reaver_MeleeAttackData;
    [SerializeField]
    private D_AttackCooldownState reaver_AttackCooldownData;

    public override void Awake()
    {
        base.Awake();

        idleState = new Reaver_IdleState(this, StateMachine, "idle", idleStateData, this);
        patrolState = new Reaver_PatrolState(this, StateMachine, "move", moveStateData, this);
        deathState = new Reaver_DeathState(this, StateMachine, "death", this);
        chasteState = new Reaver_ChaseState(this, StateMachine, "chase", chaseData);
        waitingState = new Reaver_WaitingState(this, StateMachine, "waiting", this);
        meleeAttackState = new Reaver_MeleeAttackState(this, StateMachine, "meleeAttack", reaver_MeleeAttackData, this);
        attackCooldownState = new Reaver_AttackCooldownState(this, StateMachine, "attackCooldown", reaver_AttackCooldownData);
    }

    public override State GetDeathState()
    {
        return deathState;
    }

    public override void Start()
    {
        base.Start();

        StateMachine.Initialize(patrolState);
    }
}
