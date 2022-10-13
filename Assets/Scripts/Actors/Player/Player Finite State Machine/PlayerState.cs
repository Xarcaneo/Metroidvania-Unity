using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;

public class PlayerState
{
    protected Core core;

    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

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
    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
        core = player.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        //Debug.Log(animBoolName);
        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
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