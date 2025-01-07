using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the attack state of an entity, where the entity performs an attack when the player is detected.
/// </summary>
public class AttackState : State
{
    /// <summary>
    /// Indicates whether the player has been detected by the entity.
    /// </summary>
    protected bool isPlayerDetected;

    /// <summary>
    /// The damage data associated with the entity's attack, containing relevant attack information like damage value.
    /// </summary>
    protected IDamageable.DamageData m_damageData;

    /// <summary>
    /// Gets the enemy damage hitbox component, initializing it if necessary.
    /// </summary>
    protected EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    /// <summary>
    /// Gets the movement component of the entity, initializing it if necessary.
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Gets the stats component of the entity, which holds information like attack stats.
    /// </summary>
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    /// <summary>
    /// Gets the entity detector component, which checks if the player is within detection range.
    /// </summary>
    protected EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttackState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    public AttackState(Entity entity, StateMachine stateMachine, string animBoolName)
        : base(entity, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// Performs the necessary checks during the attack state, such as detecting the player in range.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        // Check if the player is within the detection range.
        isPlayerDetected = EntityDetector.EntityInRange();
    }

    /// <summary>
    /// Called when the attack state is entered. Sets up necessary parameters for the attack, such as velocity and damage data.
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // Ensure no movement occurs during the attack state.
        Movement?.SetVelocityX(0f);

        // Set up damage data for the attack.
        m_damageData.SetData(entity, Stats.GetAttack());
    }

    /// <summary>
    /// Called during the logic update phase of the game loop. Handles logic specific to the attack state, such as keeping velocity at zero.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure no movement during the attack state.
        Movement?.SetVelocityX(0f);
    }
}
