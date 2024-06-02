using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oculon : Enemy
{
    public Oculon_RangedAttackState rangedAttackState { get; private set; }
    public Oculon_MoveState moveState { get; private set; }
    public Oculon_DeathState deathState { get; private set; }


    [SerializeField] private D_RangedAttackState rangedAttackData;
    [SerializeField] private D_AttackCooldownState attackCooldownData;
    [SerializeField] private D_MoveState moveStateData;

    [SerializeField] private Transform attackPosition;
    [SerializeField] private Transform[] targetPoints;

    public override void Awake()
    {
        base.Awake();

        rangedAttackState = new Oculon_RangedAttackState(this, StateMachine, "attack", rangedAttackData, attackPosition);
        moveState = new Oculon_MoveState(this, StateMachine, "move", moveStateData, targetPoints);
        deathState = new Oculon_DeathState(this, StateMachine, "death", this);
    }

    public override State GetDeathState()
    {
        return deathState;
    }

    public override void Start()
    {
        base.Start();

        StateMachine.Initialize(moveState);
    }
}
