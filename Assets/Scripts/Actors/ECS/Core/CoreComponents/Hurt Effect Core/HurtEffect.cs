using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Base class for implementing visual effects when an entity is hurt.
/// Provides core functionality for managing sprite effects.
/// </summary>
public abstract class HurtEffect : CoreComponent
{
    #region Protected Fields

    /// <summary>
    /// Reference to the entity's SpriteRenderer component.
    /// </summary>
    protected SpriteRenderer spriteRenderer;

    /// <summary>
    /// Duration of the hurt effect in seconds.
    /// </summary>
    [SerializeField, Tooltip("Duration of the hurt effect in seconds")]
    protected float duration = 0.2f;

    /// <summary>
    /// The currently running effect coroutine.
    /// </summary>
    protected Coroutine effectRoutine;

    #endregion

    #region Dependencies

    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes required components and event handlers.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    /// <summary>
    /// Sets up damage event subscription.
    /// </summary>
    protected virtual void Start()
    {
        DamageReceiver.OnDamage += OnDamageReceived;
    }

    /// <summary>
    /// Cleans up event subscriptions.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (damageReceiver != null)
        {
            DamageReceiver.OnDamage -= OnDamageReceived;
        }
    }

    #endregion

    #region Effect Control

    /// <summary>
    /// Handles damage received event.
    /// </summary>
    /// <param name="amount">Amount of damage received</param>
    protected virtual void OnDamageReceived(float amount)
    {
        TurnOnEffect();
    }

    /// <summary>
    /// Starts the hurt effect.
    /// </summary>
    public virtual void TurnOnEffect()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(EffectRoutine());
    }

    /// <summary>
    /// Coroutine that handles the effect animation.
    /// Override this in derived classes to implement specific effects.
    /// </summary>
    protected abstract IEnumerator EffectRoutine();

    /// <summary>
    /// Enables or disables this component.
    /// </summary>
    /// <param name="state">True to enable, false to disable</param>
    public override void EnableDisableComponent(bool state)
    {
        base.EnableDisableComponent(!state);
    }

    #endregion
}
