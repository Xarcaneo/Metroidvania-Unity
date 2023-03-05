using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : State
{

    private EnemyWeapon EnemyWeapon { get => enemyWeapon ?? core.GetCoreComponent(ref enemyWeapon); }
    private EnemyWeapon enemyWeapon;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_MeleeAttack stateData) : base(entity, stateMachine, animBoolName)
    {
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
        EnemyWeapon?.CheckMeleeAttack();
    }
}