using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : Death
{
    [Header("Essence prefab")]
    [SerializeField] private PlayerEssence m_playerEssencePref;
    [SerializeField] private float spawnOffset = 1.0f;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask solidLayer;
    [SerializeField] private Vector3 downffsetLeft = new Vector3(-2, 0f, 0);
    [SerializeField] private Vector3 downffsetRight = new Vector3(2, 0f, 0);
    [SerializeField] private Vector3 bottomOffset = new Vector3(0, -1.5f, 0);
    [SerializeField] private Vector3 topOffset = new Vector3(0, 0.5f, 0);

    public bool canSpawnEssence = true;

    public override void Die()
    {
        base.Die();

        if (canSpawnEssence)
        {
            SetUpPrefab();

            int active_slot = GameManager.Instance.currentSaveSlot;

            // Wait for a frame to ensure that any changes to the prefab's state are applied
            StartCoroutine(SaveAfterFrame(active_slot));

            canSpawnEssence = false;
        }
    }

    private void SetUpPrefab()
    {
        PlayerEssence m_prefab = Instantiate(m_playerEssencePref, new Vector3(core.Parent.transform.position.x,
            core.Parent.transform.position.y + spawnOffset, 0),
            Quaternion.identity);

        MovePlayerEssence(m_prefab);

        Renderer[] renderers = m_prefab.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        m_prefab.canInteract = false;
    }

    private void MovePlayerEssence(PlayerEssence playerEssence)
    {
        // Check if the Soul is close to a wall or edge
        RaycastHit2D hitLeft = Physics2D.Raycast(playerEssence.transform.position + topOffset, Vector2.left, 1f, solidLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(playerEssence.transform.position + topOffset, Vector2.right, 1f, solidLayer);
        RaycastHit2D hitLeftBottom = Physics2D.Raycast(playerEssence.transform.position + bottomOffset, Vector2.left, 1f, solidLayer);
        RaycastHit2D hitRightBottom = Physics2D.Raycast(playerEssence.transform.position + bottomOffset, Vector2.right, 1f, solidLayer);
        RaycastHit2D hitBottomLeft = Physics2D.Raycast(playerEssence.transform.position + downffsetLeft, Vector2.down, 3f, solidLayer);
        RaycastHit2D hitBottomRight = Physics2D.Raycast(playerEssence.transform.position + downffsetRight, Vector2.down, 3f, solidLayer);

        if (hitLeft.collider != null || hitLeftBottom.collider != null)
        {
            playerEssence.transform.position += new Vector3(1f, 0f, 0f);
        }
        else if (hitRight.collider != null || hitRightBottom.collider != null)
        {
            playerEssence.transform.position -= new Vector3(1f, 0f, 0f);
        }
        else if (hitBottomLeft.collider == null)
        {
            playerEssence.transform.position += new Vector3(1f, 0f, 0f);
        }
        else if (hitBottomRight.collider == null)
        {
            playerEssence.transform.position -= new Vector3(1f, 0f, 0f);
        }
    }

    private IEnumerator SaveAfterFrame(int active_slot)
    {
        // Wait for a frame to ensure that any changes to the prefab's state are applied
        yield return null;

        // Save the game to the specified slot
        if (SaveSystem.HasSavedGameInSlot(active_slot))
            SaveSystem.SaveToSlot(active_slot);
    }
}
