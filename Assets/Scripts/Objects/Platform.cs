using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Platform : MonoBehaviour, IPlatform
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Time in seconds before restoring platform collision after drop-through")]
    [Range(0.1f, 2f)]
    private float m_timeToRevertCollision = 0.5f;
    #endregion

    #region Private Fields
    private PlatformEffector2D m_effector;
    private EdgeCollider2D m_edgeCollider;
    private bool m_isCollisionDisabled;
    private const string PLAYER_LAYER = "Player";
    private int playerLayer;
    private int colliderMask;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_effector = GetComponent<PlatformEffector2D>();
        m_edgeCollider = GetComponent<EdgeCollider2D>();
        playerLayer = LayerMask.NameToLayer(PLAYER_LAYER);
        colliderMask = m_effector.colliderMask;

        if (!ValidateComponents())
        {
            Debug.LogError($"[{gameObject.name}] Failed to initialize required components!");
            return;
        }
    }
    #endregion

    #region Public Interface
    public void DropThrough()
    {
        if (!ValidateComponents() || m_isCollisionDisabled) 
        {
            return;
        }

        m_isCollisionDisabled = true;

        // Disable collision with player layer
        colliderMask &= ~(1 << playerLayer);
        m_effector.colliderMask = colliderMask;

        // Disable player jump temporarily
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = false;
        }

        StartCoroutine(RevertCollisionCoroutine());
    }
    #endregion

    #region Private Methods
    private bool ValidateComponents()
    {
        if (m_effector == null || m_edgeCollider == null)
        {
            Debug.LogError($"[{gameObject.name}] Missing required components!");
            return false;
        }
        return true;
    }

    private IEnumerator RevertCollisionCoroutine()
    {
        yield return new WaitForSeconds(m_timeToRevertCollision);

        // Re-enable collision with player layer
        colliderMask |= (1 << playerLayer);
        m_effector.colliderMask = colliderMask;

        // Re-enable player jump
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = true;
        }

        m_isCollisionDisabled = false;
    }
    #endregion
}
