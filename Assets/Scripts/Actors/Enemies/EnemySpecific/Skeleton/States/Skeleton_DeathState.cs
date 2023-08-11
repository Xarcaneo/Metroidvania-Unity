using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_DeathState : DeathState
{
    private readonly Skeleton enemy;
    private EnemyDeath EnemyDeath { get => enemyDeath ?? core.GetCoreComponent(ref enemyDeath); }
    private EnemyDeath enemyDeath;

    public Skeleton_DeathState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = (Skeleton)entity;
    }

    public override void Enter()
    {
        base.Enter();

        PixelCrushers.MessageSystem.SendMessage(this, "Killed", "Skeleton");
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            EnemyDeath.Die();
            enemy.gameObject.SetActive(false);
        }
    }
}
