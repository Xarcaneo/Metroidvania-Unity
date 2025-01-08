using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents the death state of an entity, where the entity transitions into a death state, stopping movement and triggering death-related events.
/// </summary>
public class DeathState : State
{
    /// <summary>
    /// The movement component of the entity, which controls its velocity and movement.
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// The enemy death component, which handles actions related to the enemy's death, such as triggering soul collection.
    /// </summary>
    protected EnemyDeath EnemyDeath { get => enemyDeath ?? core.GetCoreComponent(ref enemyDeath); }
    private EnemyDeath enemyDeath;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeathState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    public DeathState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// Performs any checks necessary during the death state, such as checking for surrounding conditions.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
    }

    /// <summary>
    /// Called when the death state is entered. Handles the actions that occur when the entity dies, such as stopping movement and triggering death-related events.
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // Trigger event for souls received upon enemy death.
        GameEvents.Instance.SoulsReceived(EnemyDeath.souls);

        // Disable components related to the enemy's ability to hurt other entities.
        core.GetCoreComponent<HurtArea>().EnableDisableComponent(false);
        core.GetCoreComponent<HurtEffect>().EnableDisableComponent(false);

        // Stop the entity's movement when it enters the death state.
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Called when exiting the death state. Can be used to clean up any resources or reset values.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Called during the logic update phase of the game loop. Handles the logic specific to the death state, such as stopping movement.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure the entity doesn't move while in the death state.
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Called during the physics update phase of the game loop. Handles the physics-specific actions for the death state.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    /// <summary>
    /// Handles changes to the active scene, deactivating the entity when the scene changes.
    /// </summary>
    /// <param name="arg0">The previous scene.</param>
    /// <param name="arg1">The new active scene.</param>
    private void ChangedActiveScene(Scene arg0, Scene arg1)
    {
        // Deactivate the entity if it is still active in the scene.
        if (entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(false);
        }
    }
}
