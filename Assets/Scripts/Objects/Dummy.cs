using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a training dummy that flashes when hit and plays a sound effect.
/// Used for combat practice and visual feedback.
/// </summary>
public class Dummy : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Sound to play when hit")]
    private string m_sfxPath = string.Empty;

    [SerializeField]
    [Tooltip("Material to use during the flash effect")]
    private Material m_flashMaterial;

    [SerializeField]
    [Tooltip("Original material to return to after flash")]
    private Material m_originalMaterial;

    [SerializeField]
    [Tooltip("Duration of the flash effect in seconds")]
    private float m_duration = 0.125f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the SpriteRenderer component
    /// </summary>
    private SpriteRenderer m_spriteRenderer;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes required components
    /// </summary>
    private void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Handles collision with other objects
    /// </summary>
    /// <param name="collision">The collider that entered the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ValidateComponents()) return;

        // Play hit sound if specified
        if (!string.IsNullOrEmpty(m_sfxPath))
        {
            FMODUnity.RuntimeManager.PlayOneShot(m_sfxPath, transform.position);
        }

        // Start flash effect
        StartCoroutine(FlashEffectCoroutine());     
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components
    /// </summary>
    private void InitializeComponents()
    {
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
        if (m_spriteRenderer == null)
        {
            Debug.LogError($"[{gameObject.name}] SpriteRenderer component is missing!");
            return false;
        }

        if (m_flashMaterial == null)
        {
            Debug.LogError($"[{gameObject.name}] Flash material is not assigned!");
            return false;
        }

        if (m_originalMaterial == null)
        {
            Debug.LogError($"[{gameObject.name}] Original material is not assigned!");
            return false;
        }

        if (m_duration <= 0)
        {
            Debug.LogError($"[{gameObject.name}] Flash duration should be greater than 0!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Coroutine that handles the flash effect animation
    /// Swaps materials temporarily to create a flash effect
    /// </summary>
    private IEnumerator FlashEffectCoroutine()
    {
        if (!ValidateComponents()) yield break;

        // Switch to flash material
        m_spriteRenderer.material = m_flashMaterial;

        // Wait for specified duration
        yield return new WaitForSeconds(m_duration);

        // Switch back to original material
        m_spriteRenderer.material = m_originalMaterial;
    }
    #endregion
}
