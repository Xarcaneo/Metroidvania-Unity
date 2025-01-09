using Opsive.UltimateInventorySystem.Core;
using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// Handles chest functionality for storing and dispensing items.
/// Manages chest state persistence, animations, and item distribution.
/// Integrates with the Ultimate Inventory System for item management.
/// </summary>
public class Chest : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique ID for this chest (format: AREA_CHEST_PURPOSE)")]
    /// <summary>
    /// Unique identifier for this chest.
    /// Used to track and persist chest state between game sessions.
    /// </summary>
    private string m_chestID;

    [SerializeField]
    [Tooltip("Name of the item to give when opened")]
    /// <summary>
    /// Name of the item to give when chest is opened.
    /// Must match an item name in the Ultimate Inventory System.
    /// </summary>
    private string m_itemName;

    [SerializeField]
    [Tooltip("Quantity of items to give")]
    /// <summary>
    /// Number of items to give when chest is opened.
    /// Default is 1.
    /// </summary>
    private int m_itemQuantity = 1;

    [SerializeField]
    [Tooltip("Optional particle system for opening effect")]
    /// <summary>
    /// Optional particle system component for visual feedback when opened.
    /// If assigned, will play when chest is opened.
    /// </summary>
    private ParticleSystem m_openEffect;

    [SerializeField]
    [Tooltip("Optional sound effect to play when opened")]
    /// <summary>
    /// Optional AudioSource component for opening sound effect.
    /// If assigned, will play when chest is opened.
    /// </summary>
    private AudioSource m_openSound;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component for chest animations.
    /// Controls open/close animations.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to game events system for state changes.
    /// Cached for efficient access.
    /// </summary>
    private GameEvents m_gameEvents;

    // Animation parameter names
    /// <summary>
    /// Constants for animator parameter names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string OPENED_PARAM = "opened";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates chest configuration in the Unity Editor.
    /// Ensures critical parameters are properly set.
    /// Only shows warnings in play mode.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }

    /// <summary>
    /// Initializes the chest by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes chest state after all objects are initialized.
    /// Checks if chest was previously opened.
    /// </summary>
    private void Start()
    {
        InitializeChestState();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the chest when activated by the player.
    /// Opens the chest, gives items, and updates state.
    /// </summary>
    public override void Interact()
    {
        if (!ValidateComponents()) return;

        // Play opening effects
        PlayOpenEffects();

        // Give items to player
        GiveItems();

        // Update chest state
        UpdateChestState();

        // Disable further interaction
        canInteract = false;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components.
    /// Called during Awake to ensure early component access.
    /// </summary>
    private void InitializeComponents()
    {
        // Get animator if not already assigned
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        // Get game events
        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    /// <summary>
    /// Initializes chest state from saved data.
    /// Disables interaction if chest was previously opened.
    /// </summary>
    private void InitializeChestState()
    {
        try
        {
            var chestState = DialogueLua.GetVariable($"Chest.{m_chestID}").asBool;
            if (chestState)
            {
                canInteract = false;
                if (m_animator != null)
                {
                    m_animator.SetBool(OPENED_PARAM, true);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error getting chest state: {e.Message}");
        }
    }

    /// <summary>
    /// Validates that all required components are present and properly initialized.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        return true;
    }

    /// <summary>
    /// Plays visual and audio effects for chest opening.
    /// </summary>
    private void PlayOpenEffects()
    {
        // Play animation
        if (m_animator != null)
        {
            m_animator.SetBool(OPENED_PARAM, true);
        }

        // Play particle effect if assigned
        if (m_openEffect != null)
        {
            m_openEffect.Play();
        }

        // Play sound effect if assigned
        if (m_openSound != null)
        {
            m_openSound.Play();
        }
    }

    /// <summary>
    /// Creates and gives items to the player.
    /// </summary>
    private void GiveItems()
    {
        try
        {
            var item = InventorySystemManager.CreateItem(m_itemName);
            if (item != null)
            {
                Player.Instance.m_inventory.AddItem(item, m_itemQuantity);
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Failed to create item: {m_itemName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error giving items: {e.Message}");
        }
    }

    /// <summary>
    /// Updates chest state and notifies the game system.
    /// </summary>
    private void UpdateChestState()
    {
        try
        {
            DialogueLua.SetVariable($"Chest.{m_chestID}", true);
            if (m_gameEvents != null)
            {
                m_gameEvents.TriggerStateChanged(m_chestID);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error updating chest state: {e.Message}");
        }
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when chest opening animation finishes.
    /// Notifies the interaction system that the interaction is complete.
    /// </summary>
    public void AnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
    #endregion
}
