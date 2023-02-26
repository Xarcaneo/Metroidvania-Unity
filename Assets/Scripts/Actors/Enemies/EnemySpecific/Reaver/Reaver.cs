 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver : Enemy
{
    public Reaver_IdleState idleState { get; private set; }
    public Reaver_MoveState moveState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;

    public override void Awake()
    {
        base.Awake();

        idleState = new Reaver_IdleState(this, StateMachine, "idle", idleStateData, this);
        moveState = new Reaver_MoveState(this, StateMachine, "move", moveStateData, this);
    }

    public override void Start()
    {
        base.Start();

        StateMachine.Initialize(moveState);
    }
}
