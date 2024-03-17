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
    public PlayerRollState RollState { get; private set; }
    public PlayerLadderClimbState LadderClimbState { get; private set; }
    public PlayerPrepareClimb PrepareClimb { get; private set; }
    public PlayerFinishClimb FinishClimb { get; private set; }
    public PlayerRecoverState RecoverState { get; private set; }
    public PlayerGroundHitState GroundHitState { get; private set; }
    public PlayerSpawnedState SpawnedState { get; private set; }
    public PlayerUseHotbarItem UseHotbarItem { get; private set; }
    public PlayerGripWallState GripWallState { get; private set; }

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
        RollState = new PlayerRollState(this, StateMachine, playerData, "roll");
        LadderClimbState = new PlayerLadderClimbState(this, StateMachine, playerData, "ladderClimb");
        PrepareClimb = new PlayerPrepareClimb(this, StateMachine, playerData, "prepareClimb");
        FinishClimb = new PlayerFinishClimb(this, StateMachine, playerData, "finishClimb");
        RecoverState = new PlayerRecoverState(this, StateMachine, playerData, "recover");
        GroundHitState = new PlayerGroundHitState(this, StateMachine, playerData, "groundHit");
        SpawnedState = new PlayerSpawnedState(this, StateMachine, playerData, "spawned");
        UseHotbarItem = new PlayerUseHotbarItem(this, StateMachine, playerData, "useHotbarItem");
        GripWallState = new PlayerGripWallState(this, StateMachine, playerData, "gripWall");
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

        StateMachine.Initialize(SpawnedState);

        CheckIfShouldFlip();

        GameEvents.Instance.PlayerSpawned();
    }
    #endregion

    #region Other Functions

    private void CheckIfShouldFlip()
    {
        if (GameManager.Instance.shouldFlipPlayer)
            Core.GetCoreComponent<Movement>().Flip();

        GameManager.Instance.shouldFlipPlayer = false;
    }
    #endregion
}