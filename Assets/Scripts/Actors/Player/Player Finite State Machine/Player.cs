using Audio;
using Opsive.UltimateInventorySystem.Equipping;
using PixelCrushers.QuestMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; protected set; }
    public PlayerDashState DashState { get; protected set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerHurtState HurtState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerCheckpointInteractionState CheckpointInteractionState { get; private set; }
    public PlayerBlockState BlockState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Components
    public Core Core { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public BoxCollider2D MovementCollider { get; private set; }
    #endregion

    #region Other Variables
    private Vector2 workspace;
    public QuestJournal questJournal;
    private IEquipper m_Equipper;
    public IEquipper Equipper => m_Equipper;
    #endregion

    #region Instance Variables
    private static Player _instance;

    public static Player Instance { get { return _instance; } }
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        Core = GetComponentInChildren<Core>();
        m_Equipper = GetComponent<IEquipper>();
        questJournal = GetComponent<QuestJournal>();

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "wallJump");
        DashState = new PlayerDashState(this, StateMachine, playerData, "dash"); ;
        CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, playerData, "crouchIdle");
        AttackState = new PlayerAttackState(this, StateMachine, playerData, "attack");
        HurtState = new PlayerHurtState(this, StateMachine, playerData, "hurt");
        DeathState = new PlayerDeathState(this, StateMachine, playerData, "death");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimbState");
        CheckpointInteractionState = new PlayerCheckpointInteractionState(this, StateMachine, playerData, "checkpointInteraction");
        BlockState = new PlayerBlockState(this, StateMachine, playerData, "block");
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        MovementCollider = GetComponent<BoxCollider2D>();

        StateMachine.Initialize(IdleState);

        CheckIfShouldFlip();

        GameEvents.Instance.PlayerSpawned();
    }
    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions
    public void SetColliderHeight(float height)
    {
        Vector2 center = MovementCollider.offset;
        workspace.Set(MovementCollider.size.x, height);

        center.y += (height - MovementCollider.size.y) / 2;

        MovementCollider.size = workspace;
        MovementCollider.offset = center;
    }

    public void SetColliderWidth(float width)
    {
        workspace.Set(width, MovementCollider.size.y);
        MovementCollider.size = workspace;
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    private void AnimationActionTrigger() => StateMachine.CurrentState.AnimationActionTrigger();

    public void PlaySound(SfxClip sfxClip)
    {
        if (sfxClip != null)
            sfxClip.AudioGroup.RaisePrimaryAudioEvent(sfxClip.AudioGroup.AudioSource, sfxClip, sfxClip.AudioConfiguration);

    }

    private void CheckIfShouldFlip()
    {
        if (GameManager.Instance.shouldFlipPlayer)
            Core.GetCoreComponent<Movement>().Flip();

        GameManager.Instance.shouldFlipPlayer = false;
    }
    #endregion
}