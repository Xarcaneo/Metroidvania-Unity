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

    private bool blockInput;

    private Weapon Weapon { get => weapon ?? core.GetCoreComponent(ref weapon); }
    private Weapon weapon;

    public PlayerAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = CollisionSenses.Ground;
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

        Movement?.SetVelocityX(0f);
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

        Movement?.SetVelocityX(velocityToSet * Movement.FacingDirection);

        blockInput = player.InputHandler.BlockInput;

        if(blockInput)
        {
            stateMachine.ChangeState(player.PrepareBlockState);
        }
        else if (!isExitingState && isAnimationFinished)
        {
            if (attackInput)
            {
                stateMachine.ChangeState(player.AttackState);
            }
            else
            {
                isAbilityDone = true;
            }
        }
    }

    private void ResetAttackCounter()
    {
        if (Time.time >= lastAttackTime + playerData.breakComboTime)
        {
            attackCounter = 0;
        }
        else
        {
            if (attackCounter > 2)
            {
                attackCounter = 0;
            }
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
        Weapon?.CheckMeleeAttack();       
    }
}

