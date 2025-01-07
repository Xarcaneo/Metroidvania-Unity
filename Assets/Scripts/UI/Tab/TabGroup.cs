using PixelCrushers.QuestMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages a group of tabs and their associated content.
/// </summary>
public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<Tab> objectsToSwap;
    private List<UITabButton> tabButtons;

    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabActive;

    private UITabButton selectedTab;
    private int index;

    private void Awake()
    {
        ValidateComponents();
    }

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

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMenuNextTab -= NextTab;
            InputManager.Instance.OnMenuPreviousTab -= PreviousTab;
        }
    }

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

    private void NextTab()
    {
        if (tabButtons == null || index + 1 >= tabButtons.Count) return;
        OnTabSelected(tabButtons[index + 1]);
    }

    private void PreviousTab()
    {
        if (tabButtons == null || index - 1 < 0) return;
        OnTabSelected(tabButtons[index - 1]);
    }

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

    public void OnTabExit(UITabButton button)
    {
        if (button != null)
        {
            ResetTabs();
        }
    }

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
