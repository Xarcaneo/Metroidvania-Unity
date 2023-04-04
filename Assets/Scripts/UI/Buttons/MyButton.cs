using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] bool muteSound = false;

    [SerializeField] private string onSelectSoundPath;
    [SerializeField] private string onPressedSoundPath;

    private void Awake()
    {
        if (onSelectSoundPath == null) onSelectSoundPath = "event:/SFX/UIEvents/ButtonFocused";
        if (onPressedSoundPath == null)  onPressedSoundPath = "event:/SFX/UIEvents/ButtonPressed";
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlaySound(onSelectSoundPath);
        OnSelectAction();
    }

    public void OnPressed()
    {
        PlaySound(onPressedSoundPath);
        OnPressedAction();
    }

    protected virtual void OnPressedAction()
    {

    }
    protected virtual void OnSelectAction()
    {
 
    }

    public void PlaySound(string path)
    {
        if (muteSound == false)
            FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);

        if (muteSound) muteSound = false;
    }
}
