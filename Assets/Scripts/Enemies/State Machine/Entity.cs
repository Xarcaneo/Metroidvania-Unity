using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public Animator anim { get; private set; }
    public Core Core { get; private set; }

    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();

        anim = GetComponent<Animator>();
        stateMachine = new FiniteStateMachine();
    }

    public void Damage(float ammount)
    {
        Debug.Log(name + " Damaged!");
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }
    private void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    private void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void AnimationActionTrigger()
    {
        //Checks what IDamageable entities intersects with weapon collider and damage them
        Core.Weapon.CheckMeleeAttack();
    }
}