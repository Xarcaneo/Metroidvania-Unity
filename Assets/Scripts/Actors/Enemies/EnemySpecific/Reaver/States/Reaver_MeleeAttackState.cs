using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_MeleeAttackState : MeleeAttackState
{
    private readonly Reaver enemy;

    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    private bool attackBlockedByDefender = false;

    public Reaver_MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_MeleeAttack stateData, Reaver enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        DamageReceiver.OnAttackBlockedByDefender += OnAttackBlockedByDefender;

        if (Movement.FacingDirection != EnemyDamageHitBox.entityToRight)
        {
            Movement.Flip();
        }

        Movement?.SetVelocityX(0f);

        m_damageData.canBlock = true; 
    }

    public override void Exit()
    {
        DamageReceiver.OnAttackBlockedByDefender -= OnAttackBlockedByDefender;

        attackBlockedByDefender = false;

        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished && !isExitingState || attackBlockedByDefender)
        {
            stateMachine.ChangeState(enemy.attackCooldownState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    private void OnAttackBlockedByDefender() => attackBlockedByDefender = true;
}