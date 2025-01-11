using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Equipping;
using PixelCrushers.QuestMachine;
using UnityEngine;

/// <summary>
/// Main player character class that manages the player's state machine, inventory, equipment, and quest systems.
/// Inherits from Entity to share common functionality with other game entities.
/// </summary>
public class Player : Entity
{
    #region State Properties
    /// <summary>
    /// Collection of all possible player states in the state machine.
    /// Each state handles specific player behavior and transitions.
    /// </summary>
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
    public PlayerLadderFinishClimbState LadderFinishClimbState { get; private set; }
    public PlayerPrepareClimb PrepareClimb { get; private set; }
    public PlayerFinishClimb FinishClimb { get; private set; }
    public PlayerRecoverState RecoverState { get; private set; }
    public PlayerGroundHitState GroundHitState { get; private set; }
    public PlayerSpawnedState SpawnedState { get; private set; }
    public PlayerUseHotbarItem UseHotbarItem { get; private set; }
    public PlayerGripWallState GripWallState { get; private set; }
    public PlayerJumpAttackState JumpAttackState { get; private set; }

    [SerializeField, Tooltip("ScriptableObject containing player configuration data")]
    private PlayerData playerData;
    #endregion

    #region Components
    /// <summary>
    /// Reference to the player's input handler component
    /// </summary>
    public PlayerInputHandler InputHandler { get; private set; }
    #endregion

    #region Game Systems
    /// <summary>
    /// Quest system component for tracking player's quests and progress
    /// </summary>
    public QuestJournal questJournal;

    /// <summary>
    /// Equipment system interface for managing player's equipped items
    /// </summary>
    private IEquipper m_Equipper;
    public IEquipper Equipper => m_Equipper;

    /// <summary>
    /// Inventory system for managing player's items
    /// </summary>
    public Inventory m_inventory;
    #endregion

    #region Singleton Pattern
    private static Player _instance;
    public static Player Instance { get { return _instance; } }
    #endregion

    #region Unity Callback Functions
    /// <summary>
    /// Initializes the player instance and all its states
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        InitializeSingleton();
        InitializeComponents();
        InitializeStates();
    }

    /// <summary>
    /// Starts the player with initial state and configuration
    /// </summary>
    public override void Start()
    {
        base.Start();
        InitializeStartupState();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    #endregion

    #region State Management
    /// <summary>
    /// Returns the death state for entity state machine
    /// </summary>
    public override State GetDeathState()
    {
        return DeathState;
    }

    /// <summary>
    /// Returns the hurt state for entity state machine
    /// </summary>
    public override State GetHurtState()
    {
        return HurtState;
    }

    /// <summary>
    /// Forces the player state to idle
    /// </summary>
    public void SetPlayerStateToIdle() => StateMachine.Initialize(IdleState);
    #endregion

    #region Initialization Methods
    private void InitializeSingleton()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void InitializeComponents()
    {
        m_Equipper = GetComponent<IEquipper>();
        questJournal = GetComponent<QuestJournal>();
        m_inventory = GetComponent<Inventory>();
        InputHandler = GetComponent<PlayerInputHandler>();
    }

    private void InitializeStates()
    {
        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "wallJump");
        DashState = new PlayerDashState(this, StateMachine, playerData, "dash");
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
        LadderFinishClimbState = new PlayerLadderFinishClimbState(this, StateMachine, playerData, "ladderFinishClimb");
        PrepareClimb = new PlayerPrepareClimb(this, StateMachine, playerData, "prepareClimb");
        FinishClimb = new PlayerFinishClimb(this, StateMachine, playerData, "finishClimb");
        RecoverState = new PlayerRecoverState(this, StateMachine, playerData, "recover");
        GroundHitState = new PlayerGroundHitState(this, StateMachine, playerData, "groundHit");
        SpawnedState = new PlayerSpawnedState(this, StateMachine, playerData, "spawned");
        UseHotbarItem = new PlayerUseHotbarItem(this, StateMachine, playerData, "useHotbarItem");
        GripWallState = new PlayerGripWallState(this, StateMachine, playerData, "gripWall");
        JumpAttackState = new PlayerJumpAttackState(this, StateMachine, playerData, "jumpAttack");
    }

    private void InitializeStartupState()
    {
        StateMachine.Initialize(SpawnedState);
        GameEvents.Instance.PlayerSpawned();
    }
    #endregion
}