using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    public Skeleton_IdleState idleState { get; private set; }
    public Skeleton_PatrolState patrolState { get; private set; }
    public Skeleton_ChaseState chaseState { get; private set; }
    public Skeleton_WaitingState waitingState { get; private set; }
    public Skeleton_DeathState deathState { get; private set; }
    public Skeleton_MeleeAttackState meleeAttackState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_ChaseState chaseStateData;

    public override void Awake()
    {
        base.Awake();

        idleState = new Skeleton_IdleState(this, StateMachine, "idle", idleStateData, this);
        patrolState = new Skeleton_PatrolState(this, StateMachine, "move", moveStateData, this);
        chaseState = new Skeleton_ChaseState(this, StateMachine, "move", chaseStateData);
        waitingState = new Skeleton_WaitingState(this, StateMachine, "idle");
        deathState = new Skeleton_DeathState(this, StateMachine, "death");
        meleeAttackState = new Skeleton_MeleeAttackState(this, StateMachine, "meleeAttack");
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
