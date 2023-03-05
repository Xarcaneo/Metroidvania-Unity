using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCooldownState : State
{
    protected float timeSinceEnteredState;
    protected readonly D_AttackCooldownState stateData;

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

        timeSinceEnteredState = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timeSinceEnteredState += Time.deltaTime;  
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
