using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_DeathState : DeathState
{
    private Reaver enemy;

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
            enemy.gameObject.SetActive(false);
        }
    }
}
