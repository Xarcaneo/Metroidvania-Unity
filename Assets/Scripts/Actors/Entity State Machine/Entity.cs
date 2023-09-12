using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    #region State Variables
    public StateMachine StateMachine { get; set; }
    #endregion

    #region Components
    public Core Core { get; set; }
    public Animator Anim { get; private set; }
    public BoxCollider2D MovementCollider { get; private set; }
    public Rigidbody2D RigidBody2D { get; private set; }
    #endregion

    #region Colliders
    [SerializeField] private BoxCollider2D m_combatCollider;

    #endregion

    #region Other Variables
    protected Vector2 workspace;
    #endregion

    #region Unity Callback Functions
    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();
        StateMachine = new StateMachine();
    }

    public virtual void Start()
    {
        RigidBody2D = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        MovementCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions
    public void SetColliderHeight(float height)
    {
        Vector2 center = MovementCollider.offset;
        workspace.Set(MovementCollider.size.x, height);

        center.y += (height - MovementCollider.size.y) / 2;

        MovementCollider.size = workspace;
        MovementCollider.offset = center;

        m_combatCollider.size = new Vector2(m_combatCollider.size.x, workspace.y);
        m_combatCollider.offset = center;
    }

    public void SetColliderWidth(float width)
    {
        Vector2 center = MovementCollider.offset;
        workspace.Set(width, MovementCollider.size.y);
        MovementCollider.size = workspace;

        // Update m_combatCollider as well
        m_combatCollider.size = new Vector2(width, m_combatCollider.size.y);
        m_combatCollider.offset = center;
    }

    protected void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    protected void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    protected void AnimationActionTrigger() => StateMachine.CurrentState.AnimationActionTrigger();
    public abstract State GetDeathState();
    public abstract State GetHurtState();
    #endregion
}
