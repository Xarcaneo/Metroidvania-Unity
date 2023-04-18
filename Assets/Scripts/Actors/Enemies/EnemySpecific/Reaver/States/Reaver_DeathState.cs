using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_DeathState : DeathState
{
    private Reaver enemy;

    private EnemyDeath EnemyDeath { get => enemyDeath ?? core.GetCoreComponent(ref enemyDeath); }
    private EnemyDeath enemyDeath;

    public Reaver_DeathState(Entity entity, StateMachine stateMachine, string animBoolName, Reaver enemy) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        PixelCrushers.MessageSystem.SendMessage(this, "Killed", "Knight");
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
