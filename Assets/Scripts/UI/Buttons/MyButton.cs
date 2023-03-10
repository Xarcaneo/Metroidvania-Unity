using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] bool muteSound = false;
    [SerializeField] SfxClip SFX_Button_Pressed;
    [SerializeField] SfxClip SFX_Button_Focused;

    public void OnSelect(BaseEventData eventData)
    {
        PlaySound(SFX_Button_Focused);
        OnSelectAction();
    }

    public void OnPressed()
    {
        PlaySound(SFX_Button_Pressed);
        OnPressedAction();
    }

    protected virtual void OnPressedAction()
    {

    }
    protected virtual void OnSelectAction()
    {
 
    }

    public void PlaySound(SfxClip sfxClip)
    {
        if (sfxClip != null && muteSound == false)
            sfxClip.AudioGroup.RaisePrimaryAudioEvent(sfxClip.AudioGroup.AudioSource, sfxClip, sfxClip.AudioConfiguration);

        if (muteSound) muteSound = false;
    }
}
