using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira_RangedAttackState : RangedAttackState
{
    private readonly Optikira enemy;

    public Optikira_RangedAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_RangedAttackState stateData, Transform attackPosition) : base(entity, stateMachine, animBoolName, stateData, attackPosition)
    {
        this.enemy = (Optikira)entity;
    }

    public override void Enter()
    {
        base.Enter();

        m_damageData.canBlock = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (EntityDetector.entityToRight != Movement.FacingDirection)
            Movement.Flip();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.attackCooldownState);
        }
    }
}
