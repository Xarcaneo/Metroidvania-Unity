using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oculon_RangedAttackState : RangedAttackState
{
    private readonly Oculon enemy;

    public Oculon_RangedAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_RangedAttackState stateData, Transform attackPosition) : base(entity, stateMachine, animBoolName, stateData, attackPosition)
    {
        this.enemy = (Oculon)entity;
    }

    public override void Enter()
    {
        base.Enter();

        m_damageData.CanBlock = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (EntityDetector.entityToRight != Movement.FacingDirection)
            Movement.Flip();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
