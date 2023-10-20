using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    protected Player player;
    protected PlayerData playerData;

    public PlayerState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
        this.playerData = playerData;
    }

    public override void Enter()
    {
        base.Enter();
        GameEvents.Instance.PlayerStateChanged(this);
    }
}