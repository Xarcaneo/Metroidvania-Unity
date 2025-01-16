using UnityEngine;

/// <summary>
/// Interface for platform interactions
/// </summary>
public interface IPlatform
{
    void DropThrough();
}

/// <summary>
/// Controls a platform that players can drop through.
/// </summary>
public class Platform : MonoBehaviour, IPlatform
{
    #region Inspector Variables
    [SerializeField]
    private PlatformEffector2D m_effector;

    [SerializeField]
    [Tooltip("Time in seconds before restoring platform collision after drop-through")]
    [Range(0.1f, 2f)]
    private float m_collisionDisableTime = 0.5f;
    #endregion

    #region Private Fields
    private bool m_isCollisionDisabled;
    private bool m_shouldRevertCollision;
    private float m_revertCollisionTime = -1f;
    private Collider2D m_collider;
    private int m_playerLayer;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (m_effector == null)
        {
            m_effector = GetComponent<PlatformEffector2D>();
        }

        m_collider = GetComponent<Collider2D>();

        m_playerLayer = LayerMask.NameToLayer("Player");

        // Configure the platform effector
        if (m_effector != null)
        {
            m_effector.useOneWay = true;
            m_effector.useOneWayGrouping = true;
            m_effector.surfaceArc = 160f;
            m_effector.useSideFriction = false;
        }
    }

    private void Update()
    {
        // Check if we should revert collision
        if (m_shouldRevertCollision && Time.time >= m_revertCollisionTime)
        {
            Debug.Log($"Reverting collision on platform: {gameObject.name}, Current time: {Time.time}, Revert time: {m_revertCollisionTime}");
            RevertCollision();
        }
    }

    private void OnEnable()
    {
        ResetCollisionState();
    }

    private void OnDisable()
    {
        ResetCollisionState();
    }
    #endregion

    #region Public Interface
    public void DropThrough()
    {
        Debug.Log($"DropThrough called on platform: {gameObject.name}");
        Debug.Log($"Platform state - Effector: {m_effector != null}, Collider: {m_collider != null}, IsCollisionDisabled: {m_isCollisionDisabled}");
        
        if (m_effector == null || m_collider == null)
        {
            Debug.LogError($"Platform {gameObject.name} missing required components!");
            return;
        }
        
        if (m_isCollisionDisabled)
        {
            Debug.LogWarning($"Platform {gameObject.name} collision already disabled!");
            return;
        }

        DisableCollision();
    }
    #endregion

    #region Private Methods
    private void DisableCollision()
    {
        Debug.Log($"Disabling collision on platform: {gameObject.name}");
        if (m_effector == null || m_collider == null)
        {
            Debug.LogError("Missing required components!");
            return;
        }

        m_isCollisionDisabled = true;
        
        // Disable both the effector and collider
        m_effector.enabled = false;
        m_collider.enabled = false;
        
        // Start timer to re-enable collision
        m_revertCollisionTime = Time.time + m_collisionDisableTime;
        m_shouldRevertCollision = true;
        
        Debug.Log($"Collision disabled, will revert at time: {m_revertCollisionTime:F2} (current: {Time.time:F2}, delay: {m_collisionDisableTime})");

        // Disable player jump temporarily
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = false;
        }
    }

    private void RevertCollision()
    {
        Debug.Log($"RevertCollision called on platform: {gameObject.name}");
        if (m_effector == null || m_collider == null)
        {
            Debug.LogError("Missing required components!");
            return;
        }

        // Re-enable both the effector and collider
        m_effector.enabled = true;
        m_collider.enabled = true;
        
        m_isCollisionDisabled = false;
        m_shouldRevertCollision = false;
        
        Debug.Log($"Collision reverted successfully on platform: {gameObject.name}");

        // Re-enable player jump
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = true;
        }
    }

    private void ResetCollisionState()
    {
        Debug.Log($"Resetting collision state on platform: {gameObject.name}");
        if (m_effector == null || m_collider == null)
        {
            Debug.LogError("Missing required components!");
            return;
        }

        m_isCollisionDisabled = false;
        m_revertCollisionTime = -1f;
        m_shouldRevertCollision = false;
        
        // Ensure components are enabled
        m_effector.enabled = true;
        m_collider.enabled = true;

        // Make sure player can jump
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = true;
        }
    }
    #endregion
}
