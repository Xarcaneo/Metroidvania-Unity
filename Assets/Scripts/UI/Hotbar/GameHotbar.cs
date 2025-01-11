using Opsive.UltimateInventorySystem.UI.Item;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages the game's hotbar system for quick item access.
/// Handles item slot selection, navigation, and usage.
/// </summary>
public class GameHotbar : MonoBehaviour
{
    private const string StatePrefix = "Hotbar.";
    private const string ActiveSlotVariable = "ActiveSlot";

    #region Serialized Fields
    /// <summary>
    /// Reference to the Ultimate Inventory System's item hotbar.
    /// </summary>
    [SerializeField] private ItemHotbar m_itemHotbar;

    /// <summary>
    /// Currently selected item slot index.
    /// </summary>
    [SerializeField] private int itemSlotIndex = 1;
    #endregion

    #region Private Fields
    /// <summary>
    /// Maximum index of available item slots.
    /// </summary>
    private int maxIndex = 0;

    /// <summary>
    /// Maps slot indices to their corresponding UI view slots.
    /// </summary>
    private Dictionary<int, ItemViewSlot> itemViewSlotDictionary = new Dictionary<int, ItemViewSlot>();

    /// <summary>
    /// Reference to the player's input handler.
    /// </summary>
    private PlayerInputHandler m_playerInputHandler;

    /// <summary>
    /// Flag to track if slots are initialized
    /// </summary>
    private bool slotsInitialized = false;
    #endregion

    /// <summary>
    /// Called when the GameObject becomes enabled and active.
    /// Initializes slots if needed, subscribes to events, and loads the hotbar state.
    /// </summary>
    private void OnEnable()
    {
        if (!slotsInitialized)
        {
            InitializeSlots();
        }
        
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onNewSession += OnNewSession;
        }

        // Always try to load state when enabled
        StartCoroutine(LoadHotbarStateAfterFrame());
    }

    /// <summary>
    /// Called when the GameObject becomes disabled or inactive.
    /// Unsubscribes from events to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onNewSession -= OnNewSession;
        }
    }

    /// <summary>
    /// Handles the start of a new game session.
    /// Reloads the hotbar state to ensure consistency.
    /// </summary>
    private void OnNewSession()
    {
        StartCoroutine(LoadHotbarStateAfterFrame());
    }

    /// <summary>
    /// Initializes the item view slots by finding and mapping all child ItemViewSlot components.
    /// Sets up the dictionary mapping slot indices to their UI components.
    /// </summary>
    private void InitializeSlots()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childTransforms)
        {
            ItemViewSlot itemViewSlot = childTransform.GetComponent<ItemViewSlot>();

            if (itemViewSlot != null)
            {
                itemViewSlotDictionary[maxIndex] = itemViewSlot;
                maxIndex++;
            }
        }

        maxIndex -= 1;
        slotsInitialized = true;
    }

    /// <summary>
    /// Coroutine that waits for the appropriate time to load the hotbar state.
    /// Ensures all necessary components and systems are initialized before loading.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator LoadHotbarStateAfterFrame()
    {
        // Wait for end of frame to ensure all components are initialized
        yield return new WaitForEndOfFrame();

        // Wait for GameEvents if needed
        while (GameEvents.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        LoadHotbarState();
    }

    /// <summary>
    /// Loads the hotbar state from DialogueLua variables.
    /// Handles cases where the variable might not exist or contain invalid values.
    /// </summary>
    private void LoadHotbarState()
    {
        string variableName = $"{StatePrefix}{ActiveSlotVariable}";
        
        if (DialogueLua.DoesVariableExist(variableName))
        {
            var luaVar = DialogueLua.GetVariable(variableName).asInt;
            if (luaVar >= 0 && luaVar <= maxIndex)
            {
                itemSlotIndex = luaVar;
            }
        }
        else
        {
            // Initialize the variable if it doesn't exist
            DialogueLua.SetVariable(variableName, itemSlotIndex);
        }
        
        FocusSlot(itemSlotIndex);
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes player input system.
    /// </summary>
    private void Start()
    {
        InitializePlayerInput();
    }

    /// <summary>
    /// Called every frame.
    /// Handles input initialization and item switching logic.
    /// </summary>
    private void Update()
    {
        if (m_playerInputHandler == null)
        {
            InitializePlayerInput();
            return;
        }

        HandleItemSwitching();
    }

    /// <summary>
    /// Uses the item in the currently selected hotbar slot.
    /// </summary>
    public void UseItem() => m_itemHotbar.UseItem(itemSlotIndex);

    /// <summary>
    /// Checks if the currently selected hotbar slot is empty.
    /// </summary>
    /// <returns>True if the selected slot contains no items, false otherwise.</returns>
    public bool IsSlotEmpty()
    {
        var item = m_itemHotbar.GetItemAt(itemSlotIndex);
        return item.Amount <= 0;
    }

    /// <summary>
    /// Initializes the player input handler by finding the player GameObject and getting its input component.
    /// </summary>
    private void InitializePlayerInput()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        m_playerInputHandler = player.GetComponent<PlayerInputHandler>();
    }

    /// <summary>
    /// Handles the logic for switching between hotbar slots based on player input.
    /// Processes both left and right switching inputs.
    /// </summary>
    private void HandleItemSwitching()
    {
        var itemSwitchLeftInput = m_playerInputHandler.ItemSwitchLeftInput;
        var itemSwitchRightInput = m_playerInputHandler.ItemSwitchRightInput;

        if (itemSwitchRightInput)
        {
            if (itemSlotIndex >= maxIndex) itemSlotIndex = 0;
            else itemSlotIndex++;

            m_playerInputHandler.UseItemSwitchRightInput();
            UpdateHotbarState();
        }

        if (itemSwitchLeftInput)
        {
            if (itemSlotIndex <= 0) itemSlotIndex = maxIndex;
            else itemSlotIndex--;

            m_playerInputHandler.UseItemSwitchLeftInput();
            UpdateHotbarState();
        }
    }

    /// <summary>
    /// Updates the hotbar state in DialogueLua and focuses the selected slot.
    /// Called after switching slots to persist the current selection.
    /// </summary>
    private void UpdateHotbarState()
    {
        DialogueLua.SetVariable($"{StatePrefix}{ActiveSlotVariable}", itemSlotIndex);
        FocusSlot(itemSlotIndex);
    }

    /// <summary>
    /// Focuses the specified hotbar slot by selecting its UI component.
    /// </summary>
    /// <param name="index">The index of the slot to focus</param>
    private void FocusSlot(int index)
    {
        if (itemViewSlotDictionary.ContainsKey(index))
        {
            ItemViewSlot itemViewSlot = itemViewSlotDictionary[index];
            itemViewSlot.Select();
        }
    }
}
