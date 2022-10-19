using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabGroup : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToSwap;
    private List<TabButton> tabButtons;

    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabActive;

    private TabButton selectedTab;
    private int index;

    protected PlayerInput menuInput;

    private void OnDisable()
    {
        int startIndex = tabButtons.Count / 2;
        OnTabSelected(tabButtons[startIndex]);
    }

    private void Start()
    {
        menuInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (menuInput.actions["PreviousTab"].triggered)
        {
            OnTabSelected(tabButtons[index - 1]);
        }
        else if (menuInput.actions["NextTab"].triggered)
        {
            OnTabSelected(tabButtons[index + 1]);
        }
    }

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
        button.background.sprite = tabIdle;
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        index = button.transform.GetSiblingIndex();
        for(int i=0; i<objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons)
        {
            button.background.sprite = tabIdle;
        }
    }
}
