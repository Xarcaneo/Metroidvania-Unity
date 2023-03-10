using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RebindButton : MyButton
{
    private Button button;
    private bool m_buttonWasFocused = false;

    [SerializeField] GameObject m_rebindingPanel;
    private bool m_isVisible;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (m_rebindingPanel.activeSelf != m_isVisible)
        {
            m_isVisible = m_rebindingPanel.activeSelf;

            if (m_isVisible)
            {
                // If the panel is enabled and the button is focused, disable the button and keep track of it
                if (button.gameObject == EventSystem.current.currentSelectedGameObject)
                {
                    m_buttonWasFocused = true;
                    button.interactable = false;
                }
                else
                {
                    m_buttonWasFocused = false;
                }
            }
            else
            {
                // If the panel is disabled and the button was focused before, enable the button and focus it
                if (m_buttonWasFocused)
                {
                    button.interactable = true;
                    button.Select();
                    m_buttonWasFocused = false;
                }
            }
        }
    }
}
