using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Button that handles input rebinding functionality.
/// Manages focus state when rebinding panel is shown/hidden.
/// </summary>
public class RebindButton : MyButton
{
    #region Private Fields
    /// <summary>
    /// Reference to the Unity Button component.
    /// </summary>
    private Button button;

    /// <summary>
    /// Tracks if this button was focused when panel opened.
    /// Used to restore focus when panel closes.
    /// </summary>
    private bool m_buttonWasFocused = false;

    /// <summary>
    /// Tracks current visibility state of rebinding panel.
    /// </summary>
    private bool m_isVisible;
    #endregion

    #region Serialized Fields
    /// <summary>
    /// Reference to the rebinding UI panel.
    /// This panel is shown when rebinding is active.
    /// </summary>
    [SerializeField] GameObject m_rebindingPanel;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Caches reference to the Button component.
    /// </summary>
    private void Start()
    {
        button = GetComponent<Button>();
    }

    /// <summary>
    /// Handles button interactability based on panel visibility.
    /// - Disables button and tracks focus when panel shows
    /// - Re-enables button and restores focus when panel hides
    /// </summary>
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
    #endregion
}
