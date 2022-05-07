using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public Animator anim { get; private set; }
    public Core Core { get; private set; }

    public EntityDetector playerDetector;

    private Vector2 velocityWorkspace;

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
}