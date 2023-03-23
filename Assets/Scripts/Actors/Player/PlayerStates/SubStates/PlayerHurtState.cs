using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtState : PlayerState
{
    //Checks
    private bool isGrounded;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private KnockbackReceiver KnockbackReceiver { get => knockbackReceiver ?? core.GetCoreComponent(ref knockbackReceiver); }
    private KnockbackReceiver knockbackReceiver;

    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    public PlayerHurtState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
        }
    }

    public override void Enter()
    {
        base.Enter();

        DamageReceiver.OnDamage += OnDamageReceived;
        KnockbackReceiver.OnKnockback += OnKnockbackReceived;

        Movement?.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();

        DamageReceiver.OnDamage -= OnDamageReceived;
        KnockbackReceiver.OnKnockback -= OnKnockbackReceived;

        KnockbackReceiver.isKnockable = true;
        DamageReceiver.isDamagable = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (isAnimationFinished && !isExitingState && !KnockbackReceiver.isKnockbackActive)
        {
            if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
            {
                stateMachine.ChangeState(player.LandState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void OnDamageReceived(float damage) => DamageReceiver.isDamagable = false;
    private void OnKnockbackReceived() => KnockbackReceiver.isKnockable = false;
}
