using UnityEngine;

/// <summary>
/// Base class for all entities in the game that use state-based behavior.
/// Provides core functionality for state management, animation, and physics.
/// </summary>
public abstract class Entity : MonoBehaviour
{
    #region State Variables
    /// <summary>
    /// Gets or sets the state machine that manages this entity's states.
    /// </summary>
    public StateMachine StateMachine { get; set; }
    #endregion

    #region Core Components
    /// <summary>
    /// Gets or sets the core component container.
    /// </summary>
    public Core Core { get; set; }

    /// <summary>
    /// Gets the animator component for handling animations.
    /// </summary>
    public Animator Anim { get; private set; }

    /// <summary>
    /// Gets the box collider used for movement collision detection.
    /// </summary>
    public BoxCollider2D MovementCollider { get; private set; }

    /// <summary>
    /// Gets the rigidbody component for physics calculations.
    /// </summary>
    public Rigidbody2D RigidBody2D { get; private set; }
    #endregion

    #region Colliders
    [SerializeField, Tooltip("Collider used for combat interactions")]
    private BoxCollider2D m_combatCollider;
    #endregion

    #region Other Variables
    /// <summary>
    /// Temporary workspace vector for calculations.
    /// </summary>
    protected Vector2 workspace;
    #endregion

    #region Unity Callback Functions
    /// <summary>
    /// Initializes core components and state machine.
    /// </summary>
    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();
        StateMachine = new StateMachine();
    }

    /// <summary>
    /// Initializes required components.
    /// </summary>
    public virtual void Start()
    {
        RigidBody2D = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        MovementCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Updates core logic and current state.
    /// </summary>
    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
    }

    /// <summary>
    /// Updates physics-based state behavior.
    /// </summary>
    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Collider Modification
    /// <summary>
    /// Adjusts the height of both movement and combat colliders.
    /// </summary>
    /// <param name="height">New height value for the colliders</param>
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

    /// <summary>
    /// Adjusts the width of both movement and combat colliders.
    /// </summary>
    /// <param name="width">New width value for the colliders</param>
    public void SetColliderWidth(float width)
    {
        Vector2 center = MovementCollider.offset;
        workspace.Set(width, MovementCollider.size.y);
        MovementCollider.size = workspace;

        m_combatCollider.size = new Vector2(width, m_combatCollider.size.y);
        m_combatCollider.offset = center;
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Triggers a general animation event in the current state.
    /// </summary>
    protected void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    /// <summary>
    /// Signals that the current animation has finished.
    /// </summary>
    protected void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    /// <summary>
    /// Triggers a specific action during an animation.
    /// </summary>
    protected void AnimationActionTrigger() => StateMachine.CurrentState.AnimationActionTrigger();
    #endregion

    #region State Getters
    /// <summary>
    /// Gets the death state for this entity.
    /// </summary>
    /// <returns>The state to transition to when the entity dies.</returns>
    public abstract State GetDeathState();

    /// <summary>
    /// Gets the hurt state for this entity.
    /// </summary>
    /// <returns>The state to transition to when the entity takes damage.</returns>
    public abstract State GetHurtState();
    #endregion
}
