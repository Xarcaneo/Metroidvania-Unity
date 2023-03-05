 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver : Enemy
{
    public Reaver_IdleState idleState { get; private set; }
    public Reaver_MoveState moveState { get; private set; }
    public Reaver_DeathState deathState { get; private set; }
    public Reaver_PlayerDetectedState playerDetectedState { get; private set; }
    public Reaver_WaitingState waitingState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_Reaver_PlayerDetectedState reaver_PlayerDetectedData;

    public override void Awake()
    {
        base.Awake();

        idleState = new Reaver_IdleState(this, StateMachine, "idle", idleStateData, this);
        moveState = new Reaver_MoveState(this, StateMachine, "move", moveStateData, this);
        deathState = new Reaver_DeathState(this, StateMachine, "death", this);
        playerDetectedState = new Reaver_PlayerDetectedState(this, StateMachine, "playerDetected", reaver_PlayerDetectedData, this);
        waitingState = new Reaver_WaitingState(this, StateMachine, "waiting", this);
    }

    public override State GetDeathState()
    {
        return deathState;
    }

    public override void Start()
    {
        base.Start();

        StateMachine.Initialize(moveState);
    }
}
