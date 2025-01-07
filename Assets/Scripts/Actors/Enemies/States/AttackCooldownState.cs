using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the attack cooldown state of an entity, typically used after an attack to wait before the entity can attack again.
/// </summary>
public class AttackCooldownState : State
{
    /// <summary>
    /// The time elapsed since entering this state.
    /// </summary>
    protected float timeSinceEnteredState;

    /// <summary>
    /// The data associated with the attack cooldown state, containing state-specific parameters such as cooldown duration.
    /// </summary>
    protected readonly D_AttackCooldownState stateData;

    /// <summary>
    /// Gets the movement component of the entity, initializing it if necessary.
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttackCooldownState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data associated with the attack cooldown state, such as cooldown duration.</param>
    public AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    /// <summary>
    /// Performs the necessary checks for detecting the player or other factors during the attack cooldown.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        // Additional checks can be added here if needed, like checking if the player is in range again
    }

    /// <summary>
    /// Called when the state is entered. Initializes the state, such as resetting the timer and setting velocity.
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // Ensure no movement during the cooldown state.
        Movement?.SetVelocityX(0f);
        timeSinceEnteredState = 0f; // Reset the cooldown timer when entering this state.
    }

    /// <summary>
    /// Called when the state is exited. Can be used to reset or clean up the state.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Called during the logic update phase of the game loop. Handles the update of the cooldown timer.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure no movement during the cooldown state.
        Movement?.SetVelocityX(0f);
        timeSinceEnteredState += Time.deltaTime;  // Increase the time since the state was entered
    }

    /// <summary>
    /// Called during the physics update phase of the game loop. Can be used to handle state-specific physics logic.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
