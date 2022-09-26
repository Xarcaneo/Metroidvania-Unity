using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : Interactable
{
    public override void Interact()
    {
        DialogueManager.StartConversation("New Conversation 1");
    }
}
