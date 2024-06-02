using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oculon_MoveState : MoveState
{
    private readonly Oculon enemy;
    private Transform[] targetPoints;

    private int targetPointIndex;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Oculon_MoveState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData, Transform[] targetPoints) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Oculon)entity;
        this.targetPoints = targetPoints;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }

    public override void PhysicsUpdate()
    {
        Vector2 targetPosition = targetPoints[targetPointIndex].position;
        Vector2 currentPosition = enemy.transform.position;

        // Determine the direction of movement along the x-axis
        int xDirection = targetPosition.x > currentPosition.x ? 1 : -1;

        if (Vector2.Distance(enemy.transform.position, targetPoints[targetPointIndex].position) < 0.02f)
        {
            targetPointIndex++;
            if (targetPointIndex == targetPoints.Length)
            {
                targetPointIndex = 0;
            }

            if (isPlayerDetected)
                stateMachine.ChangeState(enemy.rangedAttackState);
            else
                stateMachine.ChangeState(enemy.moveState);
        }

        Movement?.CheckIfShouldFlip(xDirection);
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, targetPoints[targetPointIndex].position, stateData.movementSpeed * Time.deltaTime);
    }
}
