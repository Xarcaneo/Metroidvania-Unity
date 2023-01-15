using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetector : CoreComponent
{
    protected override void Awake()
    {
        base.Awake();

        try
        {
            GameEvents.Instance.onPauseTrigger += EnableDisableComponent;
            GameEvents.Instance.onDialogueTrigger += EnableDisableComponent;
        }
        catch
        {
        }
    }

    private void OnDestroy()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger -= EnableDisableComponent;
            GameEvents.Instance.onDialogueTrigger -= EnableDisableComponent;
        }
        catch
        {
        }
    }

    public override void EnableDisableComponent(bool state)
    {
        base.EnableDisableComponent(!state);
    }
}
