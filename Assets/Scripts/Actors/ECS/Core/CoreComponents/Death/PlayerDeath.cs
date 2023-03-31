using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : Death
{
    [SerializeField] private PlayerEssence m_playerEssencePref;
    [SerializeField] private float spawnOffset = 1.0f;

    private bool canSpawnEssence = true;

    public override void Die()
    {
        base.Die();

        if (canSpawnEssence)
        {
            PlayerEssence m_prefab = Instantiate(m_playerEssencePref, new Vector3(core.Parent.transform.position.x,
                core.Parent.transform.position.y + spawnOffset, 0),
                Quaternion.identity);

            Renderer[] renderers = m_prefab.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }

            m_prefab.canInteract = false;

            int active_slot = GameManager.Instance.currentSaveSlot;

            if (SaveSystem.HasSavedGameInSlot(active_slot))
                SaveSystem.SaveToSlot(active_slot);

            canSpawnEssence = false;
        }
    }
}
