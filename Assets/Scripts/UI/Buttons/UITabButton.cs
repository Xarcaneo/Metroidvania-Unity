using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Button that represents a tab in a tabbed UI interface.
/// Manages tab selection and background image states.
/// </summary>
public class UITabButton : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Reference to the tab group this button belongs to.
    /// Controls tab switching and selection behavior.
    /// </summary>
    [SerializeField] private TabGroup tabGroup;
    #endregion

    #region Public Fields
    /// <summary>
    /// Reference to the button's background image.
    /// Used to show visual feedback for tab states.
    /// </summary>
    public Image background;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Initializes the tab button and registers with its tab group.
    /// </summary>
    private void Awake()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }
    #endregion
}
