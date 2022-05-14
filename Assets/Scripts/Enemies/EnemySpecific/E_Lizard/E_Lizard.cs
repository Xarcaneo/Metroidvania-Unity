using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard : Entity
{
    public E_Lizard_IdleState idleState { get; private set; }
    public E_Lizard_MoveState moveState { get; private set; }
    public E_Lizard_ChargeState chargeState { get; private set; }
    public E_Lizard_MeleeAttackState meleeAttackState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_ChargeState chargeStateData;
    [SerializeField]
    private D_MeleeAttack meleeAttackData;

    public override void Awake()
    {
        base.Awake();

        moveState = new E_Lizard_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new E_Lizard_IdleState(this, stateMachine, "idle", idleStateData, this);
        chargeState = new E_Lizard_ChargeState(this, stateMachine, "charge", chargeStateData, this);
        meleeAttackState = new E_Lizard_MeleeAttackState(this, stateMachine, "attack", meleeAttackData, this);

        stateMachine.Initialize(moveState);

    }
}
