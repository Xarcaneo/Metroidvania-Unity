using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : State
{
    protected bool isPlayerDetected;
    protected EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public PlayerDetectedState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }
}
