using Opsive.UltimateInventorySystem.DropsAndPickups;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a chest that can be interacted with to open and drop items.
/// </summary>
public class Chest : Interactable
{
    #region Serialized Fields
    /// <summary>
    /// Unique ID for this chest used to track its state.
    /// </summary>
    [SerializeField] private string m_ChestID;

    /// <summary>
    /// Reference to the ItemDropper component for dropping items when opened.
    /// </summary>
    [SerializeField] private ItemDropper m_ItemDropper;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the Animator component for chest animations.
    /// </summary>
    private Animator chestAnim;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the chest state and animator.
    /// </summary>
    /// <returns>An enumerator for coroutine execution.</returns>
    IEnumerator Start()
    {
        chestAnim = GetComponent<Animator>();

        yield return new WaitForEndOfFrame();
        var chestState = DialogueLua.GetVariable("Chest." + m_ChestID).asBool;

        if (chestState) SetOpened();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the chest when activated by the player.
    /// </summary>
    public override void Interact()
    {
        base.Interact();

        chestAnim.SetBool("Opening", true);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Sets the chest state to opened and disables interaction.
    /// </summary>
    private void SetOpened()
    {
        canInteract = false;
        chestAnim.SetBool("IdleOpened", true);
    }

    /// <summary>
    /// Triggers actions when the opening animation completes.
    /// </summary>
    void OnAnimationTrigger()
    {
        SetOpened();
        DialogueLua.SetVariable("Chest." + m_ChestID, true);

        m_ItemDropper.Drop();
    }
    #endregion
}
