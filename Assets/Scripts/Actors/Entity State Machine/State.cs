using UnityEngine;

/// <summary>
/// Base class for all entity states in the game.
/// Provides core functionality for state behavior, animation handling, and event management.
/// </summary>
public class State
{
    #region Core Components
    /// <summary>
    /// Reference to the core component of the entity.
    /// </summary>
    protected Core core;

    /// <summary>
    /// Reference to the entity that this state belongs to.
    /// </summary>
    protected Entity entity;

    /// <summary>
    /// Reference to the state machine that manages this state.
    /// </summary>
    protected StateMachine stateMachine;
    #endregion

    #region State Variables
    /// <summary>
    /// Flag that indicates whether the animation has finished playing.
    /// </summary>
    protected bool isAnimationFinished;

    /// <summary>
    /// Flag that indicates whether the state is currently being exited.
    /// </summary>
    protected bool isExitingState;

    /// <summary>
    /// The time that the state was entered.
    /// </summary>
    protected float startTime;

    /// <summary>
    /// The name of the animation boolean parameter.
    /// </summary>
    private readonly string animBoolName;
    #endregion

    #region Core Component References
    /// <summary>
    /// Reference to the Stats component of the entity (cached for efficiency).
    /// </summary>
    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    /// <summary>
    /// Reference to the DamageReceiver component of the entity (cached for efficiency).
    /// </summary>
    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;
    #endregion

    /// <summary>
    /// Initializes a new instance of the State class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    public State(Entity entity, StateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        core = entity.Core;
    }

    /// <summary>
    /// Destructor that unsubscribes from events when the state is destroyed.
    /// </summary>
    ~State()
    {
        DamageReceiver.OnDamage -= OnDamage;
        Stats.HealthZero -= OnHealthZero;
    }

    #region Event Management
    /// <summary>
    /// Subscribe to events when the state is entered.
    /// </summary>
    private void SubscribeEvents()
    {
        Stats.HealthZero += OnHealthZero;
        DamageReceiver.OnDamage += OnDamage;
    }

    /// <summary>
    /// Unsubscribe from events when the state is exited.
    /// </summary>
    private void UnsubscribeEvents()
    {
        Stats.HealthZero -= OnHealthZero;
        DamageReceiver.OnDamage -= OnDamage;
    }
    #endregion

    #region State Lifecycle
    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public virtual void Enter()
    {
        DoChecks();
        entity.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationFinished = false;
        isExitingState = false;
        SubscribeEvents();
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public virtual void Exit()
    {
        entity.Anim.SetBool(animBoolName, false);
        isExitingState = true;
        UnsubscribeEvents();
    }

    /// <summary>
    /// Called once per frame for logic updates.
    /// </summary>
    public virtual void LogicUpdate() { }

    /// <summary>
    /// Called once per frame for physics updates.
    /// </summary>
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    /// <summary>
    /// Perform any necessary checks before entering or updating the state.
    /// </summary>
    public virtual void DoChecks() { }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called when the animation trigger is fired.
    /// </summary>
    public virtual void AnimationTrigger() { }

    /// <summary>
    /// Called when the animation finish trigger is fired.
    /// </summary>
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;

    /// <summary>
    /// Called when the animation action trigger is fired.
    /// </summary>
    public virtual void AnimationActionTrigger() { }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Called when the entity takes damage.
    /// </summary>
    /// <param name="amount">Amount of damage taken</param>
    public virtual void OnDamage(float amount)
    {
        if (entity.StateMachine.CurrentState != entity.GetDeathState() && 
            entity.StateMachine.CurrentState != entity.GetHurtState() && 
            entity.GetHurtState() != null)
        {
            stateMachine.ChangeState(entity.GetHurtState());
        }
    }

    /// <summary>
    /// Called when the entity's health reaches zero.
    /// </summary>
    public virtual void OnHealthZero()
    {
        if (entity.GetDeathState() != null)
            stateMachine.ChangeState(entity.GetDeathState());
    }
    #endregion
}
