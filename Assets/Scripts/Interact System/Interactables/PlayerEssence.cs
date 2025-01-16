using UnityEngine;

/// <summary>
/// Handles player essence collectible functionality.
/// Represents a collectible essence that can be gathered by the player,
/// potentially providing various effects or progress tracking.
/// Also manages the transfer of souls from the player on death.
/// </summary>
public class PlayerEssence : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Sound effect to play when collected")]
    /// <summary>
    /// Optional AudioSource component for collection sound effect.
    /// If assigned, will play when essence is collected.
    /// </summary>
    private AudioSource m_collectionSound;

    [SerializeField]
    [Tooltip("Time to wait before destroying after collection")]
    /// <summary>
    /// Delay in seconds before destroying the essence object after collection.
    /// Allows time for effects to play. Default is 0.5 seconds.
    /// </summary>
    private float m_destroyDelay = 0.5f;

    [SerializeField]
    [Tooltip("Percentage of souls to take from the player on death")]
    /// <summary>
    /// Percentage of souls to be taken from the player upon death.
    /// </summary>
    private float m_soulLossPercentage = 50f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Amount of souls collected from the player on death.
    /// </summary>
    private int m_collectedSouls;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes required components
    /// </summary>
    private void Awake()
    {
        ValidateComponents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the essence when activated by the player.
    /// Triggers collection effects and destroys the essence object.
    /// </summary>
    public override void Interact()
    {
        // Play collection effects
        PlayCollectionEffects();

        // Notify any listeners about collection
        NotifyCollection();

        // Destroy with delay if effects are playing
        if (m_collectionSound != null)
        {
            canInteract = false; // Prevent multiple collections
            Destroy(gameObject, m_destroyDelay);
        }
        else
        {
            // Destroy immediately if no effects
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles the transfer of souls from the player on death.
    /// </summary>
    /// <param name="playerSouls">The total amount of souls the player has on death.</param>
    public int ExtractSoulsOnDeath(int playerSouls)
    {
        m_collectedSouls = Mathf.FloorToInt(playerSouls * (m_soulLossPercentage / 100f));
        return m_collectedSouls;
    }

    /// <summary>
    /// Gets the total amount of souls currently collected.
    /// </summary>
    /// <returns>The amount of souls collected.</returns>
    public int GetCollectedSouls()
    {
        return m_collectedSouls;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Validates that optional components are properly configured.
    /// </summary>
    private void ValidateComponents()
    {
        // Check audio source
        if (m_collectionSound != null && m_collectionSound.clip != null)
        {
            if (m_collectionSound.clip.length > m_destroyDelay)
            {
                Debug.LogWarning($"[{gameObject.name}] Audio clip length ({m_collectionSound.clip.length}s) is longer than destroy delay ({m_destroyDelay}s)!");
            }
        }
    }

    /// <summary>
    /// Plays visual and audio effects for essence collection.
    /// </summary>
    private void PlayCollectionEffects()
    {
        // Play sound effect if assigned
        if (m_collectionSound != null)
        {
            m_collectionSound.Play();
        }
    }

    /// <summary>
    /// Notifies the game system about essence collection.
    /// Override this in derived classes to implement specific collection behavior.
    /// </summary>
    protected virtual void NotifyCollection()
    {
        // Base implementation does nothing
        // Derived classes can override to implement specific collection behavior
        // such as updating player stats, quest progress, etc.
    }
    #endregion
}
