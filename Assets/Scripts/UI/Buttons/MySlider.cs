using UnityCore.AudioManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Custom slider class that provides sound feedback for selection and value changes.
/// Handles slider selection and value change events with FMOD sound integration.
/// </summary>
public class MySlider : MonoBehaviour, ISelectHandler
{
    #region Serialized Fields
    /// <summary>
    /// If true, slider will not play sounds. Resets after one use.
    /// </summary>
    [SerializeField] bool muteSound = false;

    /// <summary>
    /// Sound to play when slider is selected/focused
    /// </summary>
    [SerializeField] private AudioEventId onSelectSound = AudioEventId.UI_Button_Focus;

    /// <summary>
    /// Sound to play when slider value changes
    /// </summary>
    [SerializeField] private AudioEventId onSlideSound = AudioEventId.UI_Slider_Move;

    /// <summary>
    /// Reference to the Slider component
    /// </summary>
    private Slider slider;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Initializes default sound paths and sets up slider value change listener
    /// </summary>
    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
    #endregion

    #region UI Event Handlers
    /// <summary>
    /// Called when slider is selected/focused.
    /// Plays selection sound and triggers custom select action.
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        if (!muteSound)
        {
            AudioManager.instance.PlayUISound(onSelectSound);
        }
        OnSelectAction();
    }

    /// <summary>
    /// Called when slider value changes.
    /// Plays slide sound and triggers custom value change action.
    /// </summary>
    private void OnSliderValueChanged(float value)
    {
        if (!muteSound)
        {
            AudioManager.instance.PlayUISound(onSlideSound);
        }
        OnSlideAction(value);
    }
    #endregion

    #region Protected Virtual Methods
    /// <summary>
    /// Override this to implement custom select behavior.
    /// Called after select sound is played.
    /// </summary>
    protected virtual void OnSelectAction()
    {
    }

    /// <summary>
    /// Override this to implement custom slide behavior.
    /// Called after slide sound is played.
    /// </summary>
    protected virtual void OnSlideAction(float value)
    {
    }
    #endregion

    #region Sound Handling
    /// <summary>
    /// Plays an FMOD sound event at slider's position.
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
