using UnityCore.AudioManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Base button class that provides sound feedback and customizable actions.
/// Handles button selection and press events with FMOD sound integration.
/// </summary>
public class MyButton : MonoBehaviour, ISelectHandler
{
    #region Serialized Fields
    /// <summary>
    /// If true, button will not play sounds. Resets after one use.
    /// </summary>
    [SerializeField] bool muteSound = false;

    /// <summary>
    /// Sound to play when button is selected/focused
    /// </summary>
    [SerializeField] private AudioEventId onSelectSound = AudioEventId.UI_Button_Focus;

    /// <summary>
    /// Sound to play when button is pressed
    /// </summary>
    [SerializeField] private AudioEventId onPressedSound = AudioEventId.UI_Button_Press;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Initializes default sound paths if not set.
    /// </summary>
    private void Awake()
    {
        // No need to set defaults as they're set in the field declarations
    }
    #endregion

    #region UI Event Handlers
    /// <summary>
    /// Called when button is selected/focused.
    /// Plays selection sound and triggers custom select action.
    /// </summary>
    /// <param name="eventData">Event data from the UI system</param>
    public void OnSelect(BaseEventData eventData)
    {
        if (!muteSound)
        {
            AudioManager.instance.PlaySound(onSelectSound);
        }
        OnSelectAction();
    }

    /// <summary>
    /// Called when button is pressed.
    /// Plays press sound and triggers custom press action.
    /// </summary>
    protected virtual void OnPressed()
    {
        if (!muteSound)
        {
            AudioManager.instance.PlaySound(onPressedSound);
        }
        OnPressedAction();
    }
    #endregion

    #region Protected Virtual Methods
    /// <summary>
    /// Override this to implement custom press behavior.
    /// Called after press sound is played.
    /// </summary>
    protected virtual void OnPressedAction()
    {
    }

    /// <summary>
    /// Override this to implement custom select behavior.
    /// Called after select sound is played.
    /// </summary>
    protected virtual void OnSelectAction()
    {
    }
    #endregion

    #region Sound Handling
    /// <summary>
    /// Plays an FMOD sound event at button's position.
    /// </summary>
    /// <param name="path">FMOD event path to play</param>
    public void PlaySound(string path)
    {
        if (muteSound == false)
            FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);

        if (muteSound) muteSound = false;
    }
    #endregion
}
