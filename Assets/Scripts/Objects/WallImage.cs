using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a wall image that plays animations at random intervals.
/// Manages animation state and timing for decorative wall elements.
/// </summary>
public class WallImage : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Minimum time between animations in seconds")]
    [Range(0.1f, 10f)]
    private float m_minAnimationTime = 1f;

    [SerializeField]
    [Tooltip("Maximum time between animations in seconds")]
    [Range(0.1f, 10f)]
    private float m_maxAnimationTime = 3f;

    [SerializeField]
    [Tooltip("Animation clip to play during idle state")]
    private AnimationClip m_idleAnimation;

    [SerializeField]
    [Tooltip("Animation clip to play during active state")]
    private AnimationClip m_activeAnimation;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to the animation coroutine
    /// </summary>
    private Coroutine m_animationCoroutine;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes components and starts animation cycle
    /// </summary>
    private void Start()
    {
        InitializeComponents();
        StartAnimationCycle();
    }

    /// <summary>
    /// Cleans up coroutines when disabled
    /// </summary>
    private void OnDisable()
    {
        StopAnimationCycle();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components
    /// </summary>
    private void InitializeComponents()
    {
        m_animator = GetComponent<Animator>();
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

        if (m_idleAnimation == null)
        {
            Debug.LogError($"[{gameObject.name}] Idle animation clip is not assigned!");
            return false;
        }

        if (m_activeAnimation == null)
        {
            Debug.LogError($"[{gameObject.name}] Active animation clip is not assigned!");
            return false;
        }

        if (m_minAnimationTime > m_maxAnimationTime)
        {
            Debug.LogError($"[{gameObject.name}] Minimum animation time cannot be greater than maximum time!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Starts the animation cycle coroutine
    /// </summary>
    private void StartAnimationCycle()
    {
        if (!ValidateComponents()) return;

        StopAnimationCycle();
        m_animationCoroutine = StartCoroutine(PlayAnimationWithRandomInterval());
    }

    /// <summary>
    /// Stops the animation cycle coroutine
    /// </summary>
    private void StopAnimationCycle()
    {
        if (m_animationCoroutine != null)
        {
            StopCoroutine(m_animationCoroutine);
            m_animationCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine that handles playing animations with random intervals
    /// </summary>
    private IEnumerator PlayAnimationWithRandomInterval()
    {
        while (true)
        {
            if (!ValidateComponents()) yield break;

            // Play idle animation
            m_animator.Play(m_idleAnimation.name);

            // Wait random interval
            float randomTime = Random.Range(m_minAnimationTime, m_maxAnimationTime);
            yield return new WaitForSeconds(randomTime);

            // Play active animation
            m_animator.Play(m_activeAnimation.name);

            // Wait for active animation to complete
            yield return new WaitForSeconds(m_activeAnimation.length);
        }
    }
    #endregion
}
