using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        OnSelectAction();
    }

    public void OnPressed()
    {
        OnPressedAction();
    }

    protected virtual void OnPressedAction()
    {

    }
    protected virtual void OnSelectAction()
    {
 
    }
}
