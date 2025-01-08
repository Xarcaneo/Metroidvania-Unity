using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Button that handles resolution selection in the settings menu.
/// Triggers an event with its index when pressed.
/// </summary>
public class ResolutionButton : MyButton
{
    #region Events
    /// <summary>
    /// Event triggered when button is pressed.
    /// Passes the button's index to subscribers.
    /// </summary>
    public event Action<int> Pressed;
    #endregion

    #region Public Fields
    /// <summary>
    /// Index of this resolution button in the resolution list.
    /// Used to identify which resolution to apply when pressed.
    /// </summary>
    public int buttonIndex;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Called when button is pressed.
    /// Triggers the Pressed event with this button's index.
    /// </summary>
    protected override void OnPressedAction()
    {
        base.OnPressedAction();
        Pressed?.Invoke(buttonIndex);
    }
    #endregion
}
