using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oculon_DeathState : DeathState
{
    private Oculon enemy;

    public Oculon_DeathState(Entity entity, StateMachine stateMachine, string animBoolName, Oculon enemy) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
    }
    public override void Enter()
    {
        base.Enter();

        PixelCrushers.MessageSystem.SendMessage(this, "Killed", "Oculon");
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
