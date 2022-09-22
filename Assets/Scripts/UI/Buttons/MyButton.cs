using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MyButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] bool muteSound = false;
    [SerializeField] SfxClip SFX_Button_Pressed;
    [SerializeField] SfxClip SFX_Button_Focused;

    public void OnSelect(BaseEventData eventData)
    {
        PlaySound(SFX_Button_Focused);
    }

    public void OnPressed()
    {
        PlaySound(SFX_Button_Pressed);
        OnPressedAction();
    }

    protected virtual void OnPressedAction()
    {

    }

    public void PlaySound(SfxClip sfxClip)
    {
        if (sfxClip != null && muteSound == false)
            sfxClip.AudioGroup.RaisePrimaryAudioEvent(sfxClip.AudioGroup.AudioSource, sfxClip, sfxClip.AudioConfiguration);

        if (muteSound) muteSound = false;
    }
}
