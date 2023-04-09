using Opsive.UltimateInventorySystem.Equipping;
using PixelCrushers.QuestMachine;
using UnityEngine;

public class Player : Entity
{
    #region State Variables

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
    public PlayerPrepareBlockState PrepareBlockState { get; private set; }
    public PlayerBlockState BlockState { get; private set; }
    public PlayerCounterAttackState CounterAttackState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Components
    public PlayerInputHandler InputHandler { get; private set; }
    #endregion

    #region Other Variables
    public QuestJournal questJournal;
    private IEquipper m_Equipper;
    public IEquipper Equipper => m_Equipper;
    #endregion

    #region Instance Variables
    private static Player _instance;

    public static Player Instance { get { return _instance; } }
    #endregion

    #region Unity Callback Functions
    public override void Awake()
    {
        base.Awake();

        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        m_Equipper = GetComponent<IEquipper>();
        questJournal = GetComponent<QuestJournal>();

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
        PrepareBlockState = new PlayerPrepareBlockState(this, StateMachine, playerData, "prepareBlock");
        BlockState = new PlayerBlockState(this, StateMachine, playerData, "block");
        CounterAttackState = new PlayerCounterAttackState(this, StateMachine, playerData, "counterAttack");
    }

    public override State GetDeathState()
    {
        return DeathState;
    }

    public override State GetHurtState()
    {
        return HurtState;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public override void Start()
    {
        base.Start();

        InputHandler = GetComponent<PlayerInputHandler>();

        StateMachine.Initialize(IdleState);

        CheckIfShouldFlip();

        GameEvents.Instance.PlayerSpawned();
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

    private void CheckIfShouldFlip()
    {
        if (GameManager.Instance.shouldFlipPlayer)
            Core.GetCoreComponent<Movement>().Flip();

        GameManager.Instance.shouldFlipPlayer = false;
    }
    #endregion
}