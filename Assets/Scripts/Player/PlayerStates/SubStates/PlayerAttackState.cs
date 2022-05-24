using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private int attackCounter;

    private float velocityToSet;

    private float lastAttackTime;

    private bool isGrounded;

    private bool attackInput;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = core.CollisionSenses.Ground;
    }

    public override void Enter()
    {
        base.Enter();

        attackInput = false;

        player.InputHandler.UseAttackInput();

        ResetAttackCounter();

        lastAttackTime = Time.time;

        SettAttackVelocity();

        player.Anim.SetInteger("attackCounter", attackCounter);
    }

    public override void Exit()
    {
        base.Exit();

        core.Movement.SetVelocityX(0f);
        velocityToSet = 0;

        attackCounter++;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(player.InputHandler.AttackInput)
        {
            attackInput = true;
        }

        core.Movement.SetVelocityX(velocityToSet * core.Movement.FacingDirection);

        if (!isExitingState)
        {
            if (isAnimationFinished && attackInput)
            {
                stateMachine.ChangeState(player.AttackState);
            }
            else if (isAnimationFinished && !attackInput)
            {
                isAbilityDone = true;
            }
        }
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

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();
      
        //Checks what IDamageable entities intersects with weapon collider and damage them
        core.Weapon.CheckMeleeAttack();       
    }
}

