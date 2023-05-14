using UnityEngine;

public class PlayerLadderClimbState : PlayerState
{
    private int yInput;
    private bool JumpInput;

    private bool isGrounded;
    private bool isTouchingLadderTop;

    private float lastJumpTime;

    private Animator m_anim;
    private Rigidbody2D m_rigidbody;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    public PlayerLadderClimbState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        m_anim = player.GetComponent<Animator>();
        m_rigidbody = player.GetComponent<Rigidbody2D>();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingLadderTop = collisionSenses.LadderTop;
        }
    }

    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();

        m_anim.speed = 0;
        m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void Exit()
    {
        base.Exit();

        m_anim.speed = 1;
        m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
        Movement?.SetVelocityY(0f);

        yInput = player.InputHandler.NormInputY;
        JumpInput = player.InputHandler.JumpInput;

        if(!isTouchingLadderTop && yInput == 1)
        {
            stateMachine.ChangeState(player.FinishClimb);
        }
        else if (JumpInput && player.JumpState.CanJump() && CheckIfCanJump())
        {
            Movement?.SetVelocityX(0f);
            lastJumpTime = Time.time;
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isGrounded && yInput == -1)
        {
            stateMachine.ChangeState(player.FinishClimb);
        }
        else if (yInput != 0)
        {
            m_anim.speed = 1;
            Movement?.SetVelocityY(playerData.climbingVelocity * yInput);;
        }
        else
            m_anim.speed = 0;
    }

    public bool CheckIfCanJump()
    {
        return Time.time >= lastJumpTime + playerData.ladderJumpCooldown;
    }
}
