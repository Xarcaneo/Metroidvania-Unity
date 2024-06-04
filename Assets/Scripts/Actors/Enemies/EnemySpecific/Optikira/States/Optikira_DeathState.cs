using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira_DeathState : DeathState
{
    private Optikira enemy;

    public Optikira_DeathState(Entity entity, StateMachine stateMachine, string animBoolName, Optikira enemy) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        PixelCrushers.MessageSystem.SendMessage(this, "Killed", "Optikira");
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
