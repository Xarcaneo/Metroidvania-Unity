using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private float velocityToSet;
    private bool blockInput;

    private DamageHitBox DamageHitBox { get => damageHitBox ?? core.GetCoreComponent(ref damageHitBox); }
    private DamageHitBox damageHitBox;
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    private IDamageable.DamageData m_damageData;

    public PlayerAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        m_damageData.SetData(player, Stats.GetAttack());
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

        if (isOnSlope)
        {
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
        }
        else
        {
            player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }

        if (blockInput)
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
        DamageHitBox?.MeleeAttack(m_damageData);
        DamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);
    }
}

