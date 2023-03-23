using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Core core; // Reference to the core component of the entity

    protected Entity entity; // Reference to the entity that this state belongs to
    protected StateMachine stateMachine; // Reference to the state machine that this state belongs to

    protected bool isAnimationFinished; // Flag that indicates whether the animation has finished playing
    protected bool isExitingState; // Flag that indicates whether the state is currently being exited

    protected float startTime; // The time that the state was entered

    private string animBoolName; // The name of the animation boolean parameter

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); } // Reference to the Stats component of the entity
    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); } // Reference to the Combat component of the entity

    private Stats stats; // Reference to the Stats component of the entity (cached for efficiency)
    private DamageReceiver damageReceiver; // Reference to the Combat component of the entity (cached for efficiency)

    public State(Entity entity, StateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        core = entity.Core;
    }

    ~State() // Destructor that unsubscribes from events when the state is destroyed
    {
        DamageReceiver.OnDamage -= OnDamage;
        Stats.HealthZero -= OnHealthZero;
    }

    // Subscribe to events when the state is entered
    private void SubscribeEvents()
    {
        Stats.HealthZero += OnHealthZero;
        DamageReceiver.OnDamage += OnDamage;
    }

    // Unsubscribe from events when the state is exited
    private void UnsubscribeEvents()
    {
        Stats.HealthZero -= OnHealthZero;
        DamageReceiver.OnDamage -= OnDamage;
    }

    public virtual void Enter() // Called when the state is entered
    {
        DoChecks(); // Perform any necessary checks before entering the state
        entity.Anim.SetBool(animBoolName, true); // Set the animation boolean parameter to true
        startTime = Time.time; // Record the current time
        isAnimationFinished = false; // Reset the animation finished flag
        isExitingState = false; // Reset the exiting state flag

        // Subscribe to events
        SubscribeEvents();
    }

    public virtual void Exit() // Called when the state is exited
    {
        entity.Anim.SetBool(animBoolName, false); // Set the animation boolean parameter to false
        isExitingState = true; // Set the exiting state flag to true

        // Unsubscribe from events
        UnsubscribeEvents();
    }

    public virtual void LogicUpdate() // Called once per frame for logic updates
    {

    }

    public virtual void PhysicsUpdate() // Called once per frame for physics updates
    {
        DoChecks(); // Perform any necessary checks before updating the physics
    }

    public virtual void DoChecks() { } // Perform any necessary checks before entering the state

    public virtual void AnimationTrigger() { } // Called when the animation trigger is fired

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true; // Called when the animation finish trigger is fired

    public virtual void AnimationActionTrigger() { } // Called when the animation action trigger is fired

    public virtual void OnDamage(float amount) // Called when the entity takes damage
    {
        if (entity.StateMachine.CurrentState != entity.GetDeathState() && entity.GetHurtState() != null)
            stateMachine.ChangeState(entity.GetHurtState());
    }

    public virtual void OnHealthZero() // Called when the entity's health reaches zero
    {
        if (entity.GetDeathState() != null)
            stateMachine.ChangeState(entity.GetDeathState());
    }
}
