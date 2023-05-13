using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira : Enemy
{
    public Optikira_IdleState idleState { get; private set; }
    public Optikira_DeathState deathState { get; private set; }
    public Optikira_RangedAttackState rangedAttackState { get; private set; }
    public Optikira_AttackCooldownState attackCooldownState { get; private set; }

    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_RangedAttackState rangedAttackData;
    [SerializeField] private D_AttackCooldownState attackCooldownData;

    [SerializeField] private Transform attackPosition;

    public override void Awake()
    {
        base.Awake();

        idleState = new Optikira_IdleState(this, StateMachine, "idle", idleStateData, this);
        deathState = new Optikira_DeathState(this, StateMachine, "death", this);
        rangedAttackState = new Optikira_RangedAttackState(this, StateMachine, "attack", rangedAttackData, attackPosition);
        attackCooldownState = new Optikira_AttackCooldownState(this, StateMachine, "attackCooldown", attackCooldownData);
    }
    public override State GetDeathState()
    {
        return deathState;
    }


    public override void Start()
    {
        base.Start();

        StateMachine.Initialize(idleState);
    }
}
