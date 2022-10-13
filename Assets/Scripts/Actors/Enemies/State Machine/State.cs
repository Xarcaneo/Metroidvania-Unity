using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State
{
    protected FiniteStateMachine stateMachine;
    protected Entity entity;
    protected Core core;

    //Events
    protected Action<float> damageEventHandler;
    protected Action healthZeroEventHandler;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;

    protected string animBoolName;

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Combat Combat { get => combat ?? core.GetCoreComponent(ref combat); }

    private Stats stats;
    private Combat combat;

    public State(Entity entity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        core = entity.Core;
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        entity.anim.SetBool(animBoolName, true);
        isAnimationFinished = false;
        isExitingState = false;

        DoChecks();
    }

    public virtual void Exit()
    {
        entity.anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;

    public virtual void AnimationActionTrigger() { }

    public void SubscribeEvents()
    {
        Combat.OnDamage += damageEventHandler;
        Stats.HealthZero += healthZeroEventHandler;
    }

    public void UnsubscribeEvents()
    {
        Combat.OnDamage -= damageEventHandler;
        Stats.HealthZero -= healthZeroEventHandler;
    }
}
