using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    protected D_MeleeAttack stateData;

    public MeleeAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MeleeAttack stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        //Checks what IDamageable entities intersects with weapon collider and damage them
        core.Weapon.CheckMeleeAttack();
    }
}