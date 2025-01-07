using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the chase state of an entity, where the entity actively chases the player when detected.
/// </summary>
public class ChaseState : State
{
    /// <summary>
    /// The data associated with the chase state, containing parameters like detection range and behavior.
    /// </summary>
    protected readonly D_ChaseState stateData;

    /// <summary>
    /// Indicates whether the player has been detected by the entity.
    /// </summary>
    protected bool isPlayerDetected;

    /// <summary>
    /// Gets the entity detector component of the entity, initializing it if necessary.
    /// </summary>
    protected EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaseState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data associated with the chase state, containing chase parameters like detection range.</param>
    public ChaseState(Entity entity, StateMachine stateMachine, string animBoolName, D_ChaseState stateData)
        : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    /// <summary>
    /// Performs the necessary checks during the chase state, such as checking if the player is in range.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        // Check if the player is within the detection range.
        isPlayerDetected = EntityDetector.EntityInRange();
    }
}
