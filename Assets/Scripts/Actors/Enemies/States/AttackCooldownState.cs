using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCooldownState : State
{
    protected float timeSinceEnteredState;
    protected readonly D_AttackCooldownState stateData;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData) : base(entity, stateMachine, animBoolName)
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

        Movement?.SetVelocityX(0f);
        timeSinceEnteredState = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
        timeSinceEnteredState += Time.deltaTime;  
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
