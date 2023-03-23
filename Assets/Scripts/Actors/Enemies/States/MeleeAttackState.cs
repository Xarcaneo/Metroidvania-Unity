using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : State
{

    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    private IDamageable.DamageData m_damageData;

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

        Movement?.SetVelocityX(0f);

        m_damageData.SetData(entity, Stats.GetAttack());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f); Movement?.SetVelocityX(0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        //Checks what IDamageable entities intersects with weapon collider and damage them
        EnemyDamageHitBox?.MeleeAttack(m_damageData);
        EnemyDamageHitBox?.Knockback(Movement.FacingDirection);
    }
}