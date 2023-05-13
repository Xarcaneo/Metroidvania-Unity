using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected bool isPlayerDetected;

    protected IDamageable.DamageData m_damageData;

    protected EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    protected EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public AttackState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }

    public override void Enter()
    {
        base.Enter();

        Movement?.SetVelocityX(0f);
        m_damageData.SetData(entity, Stats.GetAttack());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
    }


}