using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private int attackCounter;

    private float velocityToSet;

    private float lastAttackTime;

    private bool isGrounded;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        player.InputHandler.AttackInput = false;

        ResetAttackCounter();

        lastAttackTime = Time.time;

        SettAttackVelocity();

        player.Anim.SetBool("attack", true);

        player.Anim.SetInteger("attackCounter", attackCounter);
    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocityX(0f);
        velocityToSet = 0;

        player.Anim.SetBool("attack", false);

        attackCounter++;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.SetVelocityX(velocityToSet * player.FacingDirection);     
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

    private void ResetAttackCounter()
    {
        if (isGrounded)
        {
            if (Time.time >= lastAttackTime + playerData.breakComboTime)
                attackCounter = 0;
            else
            {
                if (attackCounter > 2)
                {
                    attackCounter = 0;
                }
            }
        }
        else
        {
            attackCounter = 0;
        }
    }

    private void SettAttackVelocity()
    {
        if(isGrounded)
        {
            velocityToSet = playerData.attackMovementSpeed[attackCounter];
        }
        else
        {
            velocityToSet = 0;
        }
    }
}
