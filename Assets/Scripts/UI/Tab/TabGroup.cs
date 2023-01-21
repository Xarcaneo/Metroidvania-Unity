using PixelCrushers.QuestMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabGroup : MonoBehaviour
{
    [SerializeField] List<Tab> objectsToSwap;
    private List<TabButton> tabButtons;

    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabActive;

    private TabButton selectedTab;
    private int index;

    private void OnEnable()
    {
        InputManager.Instance.OnMenuNextTab += NextTab;
        InputManager.Instance.OnMenuPreviousTab += PreviousTab;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMenuNextTab -= NextTab;
        InputManager.Instance.OnMenuPreviousTab -= PreviousTab;
    }

    public void ResetPages()
    {
        int startIndex = tabButtons.Count / 2;
        OnTabSelected(tabButtons[startIndex]);
    }

    private void NextTab()
    {
        if (index + 1 < tabButtons.Count) OnTabSelected(tabButtons[index + 1]);
    }
    private void PreviousTab()
    {
        if (index -1 >= 0)  OnTabSelected(tabButtons[index - 1]);
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
                objectsToSwap[i].gameObject.SetActive(true);
                objectsToSwap[i].OnActive();
            }
            else
            {
                objectsToSwap[i].gameObject.SetActive(false);
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
