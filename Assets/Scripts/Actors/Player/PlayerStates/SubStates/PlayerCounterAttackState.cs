using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private Weapon Weapon { get => weapon ?? core.GetCoreComponent(ref weapon); }
    private Weapon weapon;

    public PlayerCounterAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.InputHandler.UseAttackInput();
    }

    public override void Exit()
    {
        base.Exit();

        Movement?.SetVelocityX(0f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        //Checks what IDamageable entities intersects with weapon collider and damage them
        Weapon?.CheckMeleeAttack();
    }
}
