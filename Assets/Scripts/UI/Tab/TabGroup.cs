using PixelCrushers.QuestMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityCore.AudioManager;

/// <summary>
/// Manages a group of tabs and their associated content, handling tab switching and visual states.
/// </summary>
public class TabGroup : MonoBehaviour
{
    [Header("Tab Settings")]
    [Tooltip("List of tab content objects that will be shown/hidden")]
    [SerializeField] private List<Tab> objectsToSwap;

    [Header("Visual Settings")]
    [Tooltip("Sprite used for inactive tab buttons")]
    [SerializeField] private Sprite tabIdle;
    [Tooltip("Sprite used for the active tab button")]
    [SerializeField] private Sprite tabActive;

    private List<UITabButton> tabButtons;
    private UITabButton selectedTab;
    private int index;

    /// <summary>
    /// Initializes and validates required components.
    /// </summary>
    private void Awake()
    {
        ValidateComponents();
    }

    /// <summary>
    /// Subscribes to input events for tab navigation.
    /// </summary>
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMenuNextTab += NextTab;
            InputManager.Instance.OnMenuPreviousTab += PreviousTab;
        }
        else
        {
            Debug.LogWarning($"[TabGroup] InputManager.Instance is null in {gameObject.name}", this);
        }
    }

    /// <summary>
    /// Unsubscribes from input events.
    /// </summary>
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMenuNextTab -= NextTab;
            InputManager.Instance.OnMenuPreviousTab -= PreviousTab;
        }
    }

    /// <summary>
    /// Validates that all required components and references are properly set up.
    /// </summary>
    private void ValidateComponents()
    {
        if (objectsToSwap == null || objectsToSwap.Count == 0)
        {
            Debug.LogWarning($"[TabGroup] No objects to swap assigned in {gameObject.name}", this);
        }

        if (tabIdle == null)
        {
            Debug.LogWarning($"[TabGroup] Tab idle sprite not assigned in {gameObject.name}", this);
        }

        if (tabActive == null)
        {
            Debug.LogWarning($"[TabGroup] Tab active sprite not assigned in {gameObject.name}", this);
        }

        tabButtons = new List<UITabButton>();
    }

    /// <summary>
    /// Resets the tab group to show the middle tab.
    /// </summary>
    public void ResetPages()
    {
        if (tabButtons == null || tabButtons.Count == 0)
        {
            Debug.LogWarning($"[TabGroup] No tab buttons available to reset pages in {gameObject.name}", this);
            return;
        }

        int startIndex = tabButtons.Count / 2;
        OnTabSelected(tabButtons[startIndex]);
    }

    /// <summary>
    /// Switches to the next tab if available.
    /// </summary>
    private void NextTab()
    {
        if (tabButtons == null || index + 1 >= tabButtons.Count) return;
        AudioManager.instance.PlaySound(AudioEventId.UI_Tab_Next);
        OnTabSelected(tabButtons[index + 1]);
    }

    /// <summary>
    /// Switches to the previous tab if available.
    /// </summary>
    private void PreviousTab()
    {
        if (tabButtons == null || index - 1 < 0) return;
        AudioManager.instance.PlaySound(AudioEventId.UI_Tab_Next);
        OnTabSelected(tabButtons[index - 1]);
    }

    /// <summary>
    /// Registers a new tab button with this group.
    /// </summary>
    /// <param name="button">The tab button to register</param>
    public void Subscribe(UITabButton button)
    {
        if (button == null)
        {
            Debug.LogError($"[TabGroup] Attempting to subscribe null button in {gameObject.name}", this);
            return;
        }

        if (tabButtons == null)
        {
            tabButtons = new List<UITabButton>();
        }

        if (!tabButtons.Contains(button))
        {
            tabButtons.Add(button);
            if (button.background != null && tabIdle != null)
            {
                button.background.sprite = tabIdle;
            }
        }
    }

    /// <summary>
    /// Called when a tab is exited.
    /// </summary>
    /// <param name="button">The tab button that was exited</param>
    public void OnTabExit(UITabButton button)
    {
        if (button != null)
        {
            ResetTabs();
        }
    }

    /// <summary>
    /// Called when a tab is selected.
    /// </summary>
    /// <param name="button">The tab button that was selected</param>
    public void OnTabSelected(UITabButton button)
    {
        if (button == null) return;

        selectedTab = button;
        ResetTabs();

        if (button.background != null && tabActive != null)
        {
            button.background.sprite = tabActive;
        }

        index = button.transform.GetSiblingIndex();

        if (objectsToSwap != null)
        {
            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                if (objectsToSwap[i] == null) continue;

                if (i == index)
                {
                    objectsToSwap[i].gameObject.SetActive(true);
                    objectsToSwap[i].OnActive();
                }
                else
                {
                    objectsToSwap[i].gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Resets all tabs to their inactive state.
    /// </summary>
    public void ResetTabs()
    {
        if (tabButtons == null) return;

        foreach (UITabButton button in tabButtons)
        {
            if (button != null && button.background != null && tabIdle != null)
            {
                button.background.sprite = tabIdle;
            }
        }
    }
}
