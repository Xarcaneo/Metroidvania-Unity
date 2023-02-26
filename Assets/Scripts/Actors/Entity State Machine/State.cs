using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected Core core;

    protected Entity entity;
    protected StateMachine stateMachine;

    //Events
    protected Action<float> damageEventHandler;
    protected Action healthZeroEventHandler;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;

    private string animBoolName;

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Combat Combat { get => combat ?? core.GetCoreComponent(ref combat); }

    private Stats stats;
    private Combat combat;

    public State(Entity entity, StateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        core = entity.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
        entity.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        //Debug.Log(animBoolName);
        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        entity.Anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks() { }

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