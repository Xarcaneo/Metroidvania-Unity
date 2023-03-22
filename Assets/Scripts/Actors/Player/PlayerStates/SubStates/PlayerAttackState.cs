using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private float velocityToSet;
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
        SettAttackVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        Movement?.SetVelocityX(0f);
        velocityToSet = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(velocityToSet * Movement.FacingDirection);

        blockInput = player.InputHandler.BlockInput;

        if(blockInput)
        {
            stateMachine.ChangeState(player.PrepareBlockState);
        }
        else if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
            isAbilityDone = true;       
        }
    }
    
    private void SettAttackVelocity()
    {
        if(isGrounded)
        {
            velocityToSet = playerData.attackMovementSpeed[0];
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

