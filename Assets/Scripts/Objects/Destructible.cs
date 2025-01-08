using UnityEngine;

/// <summary>
/// Controls a destructible object that can be damaged and destroyed.
/// Implements the IDamageable interface for damage handling.
/// </summary>
public class Destructible : MonoBehaviour, IDamageable
{
    #region Private Fields
    /// <summary>
    /// Flag indicating if the object has been destroyed
    /// </summary>
    private bool m_isDestroyed;

    /// <summary>
    /// Reference to the animator component
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to the sprite renderer component
    /// </summary>
    private SpriteRenderer m_spriteRenderer;

    // Animation parameter names
    private const string DESTROY_PARAM = "Destroy";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes required components
    /// </summary>
    private void Start()
    {
        InitializeComponents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles damage received by the object
    /// </summary>
    /// <param name="damageData">Data about the damage received</param>
    public void Damage(IDamageable.DamageData damageData)
    {
        if (!ValidateComponents()) return;

        if (!m_isDestroyed)
        {
            m_isDestroyed = true;
            m_animator.SetBool(DESTROY_PARAM, true);
        }
    }

    /// <summary>
    /// Handles instant kill effects
    /// Currently not implemented for destructible objects
    /// </summary>
    public void InstantKill() { }

    /// <summary>
    /// Called by animation event when destruction animation finishes
    /// Destroys the game object
    /// </summary>
    public void AnimationFinishTrigger()
    {
        if (!ValidateComponents()) return;

        Destroy(gameObject);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components
    /// </summary>
    private void InitializeComponents()
    {
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        if (!ValidateComponents())
        {
            Debug.LogError($"[{gameObject.name}] Failed to initialize required components!");
        }
    }

    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        if (m_spriteRenderer == null)
        {
            Debug.LogError($"[{gameObject.name}] SpriteRenderer component is missing!");
            return false;
        }

        return true;
    }
    #endregion
}
