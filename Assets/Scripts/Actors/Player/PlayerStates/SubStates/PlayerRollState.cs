using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerAbilityState
{
    private float lastRollTime;
    private bool startRoll = false;

    protected DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    protected KnockbackReceiver KnockbackReceiver { get => knockbackReceiver ?? core.GetCoreComponent(ref knockbackReceiver); }
    private KnockbackReceiver knockbackReceiver;

    public PlayerRollState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        DamageReceiver.isDamagable = false;
        KnockbackReceiver.isKnockable = false;

        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();

        DamageReceiver.isDamagable = true;
        KnockbackReceiver.isKnockable = true;
        startRoll = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        float rollTimeElapsed = Time.time - startTime;

        if (rollTimeElapsed >= playerData.rollDuration)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        if (startRoll)
            Movement?.SetVelocityX(playerData.rollSpeed * Movement.FacingDirection);
        else
            Movement?.SetVelocityX(0f);
    }

    public bool CheckIfCanRoll ()
    {
        return Time.time >= lastRollTime + playerData.rollCooldown;
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        startRoll = true;
    }
}
