using Opsive.UltimateInventorySystem.Core;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class Lock : Interactable
{
    [SerializeField] private int m_lockID;
    [SerializeField] private string m_itemName;
    [SerializeField] private Animator m_anim;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        var lockState = DialogueLua.GetVariable("Trigger." + m_lockID).asBool;

        if (lockState)
            canInteract = false;
    }

    public override void Interact()
    {
        if (Player.Instance)
        {
            var item = InventorySystemManager.CreateItem(m_itemName);

            if (item != null)
            {
                var hasItem = Player.Instance.m_inventory.HasItem(item, false);

                if (hasItem)
                {
                    DialogueLua.SetVariable("Trigger." + m_lockID, true);
                    GameEvents.Instance.TriggerStateChanged(m_lockID);
                }
                else
                {
                    m_anim.Play("NoKeyAnimation");
                }
            }
            else
                return;
        }
    }

    void OnAnimationFinished() => CallInteractionCompletedEvent();
}
