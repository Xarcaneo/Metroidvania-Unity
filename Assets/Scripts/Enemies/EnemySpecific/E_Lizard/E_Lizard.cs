using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard : Entity
{
    public E_Lizard_IdleState idleState { get; private set; }
    public E_Lizard_MoveState moveState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;

    public override void Start()
    {
        base.Start();

        moveState = new E_Lizard_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new E_Lizard_IdleState(this, stateMachine, "idle", idleStateData, this);

        stateMachine.Initialize(moveState);

    }
}
