using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttackState : PlayerInAirState 
{
    //Input
    private int xInput;

    private IDamageable.DamageData m_damageData;

    private DamageHitBox DamageHitBox { get => damageHitBox ?? core.GetCoreComponent(ref damageHitBox); }
    private DamageHitBox damageHitBox;
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    public PlayerJumpAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        m_damageData.SetData(player, Stats.GetAttack());
        player.InputHandler.UseAttackInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState && isAnimationFinished) 
        {
            stateMachine.ChangeState(player.InAirState);
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
