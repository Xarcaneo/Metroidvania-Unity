using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player-specific death behavior and respawn mechanics.
/// </summary>
public class PlayerDeath : Death
{
    #region Essence Prefab

    /// <summary>
    /// The prefab for the player's essence.
    /// </summary>
    [SerializeField] private PlayerEssence m_playerEssencePref;

    /// <summary>
    /// The offset from the player's position where the essence will spawn.
    /// </summary>
    [SerializeField] private float spawnOffset = 1.0f;

    #endregion

    #region Raycast Settings

    /// <summary>
    /// The layer mask for solid objects.
    /// </summary>
    [SerializeField] private LayerMask solidLayer;

    /// <summary>
    /// The offset from the player's position for the left raycast.
    /// </summary>
    [SerializeField] private Vector3 downffsetLeft = new Vector3(-2, 0f, 0);

    /// <summary>
    /// The offset from the player's position for the right raycast.
    /// </summary>
    [SerializeField] private Vector3 downffsetRight = new Vector3(2, 0f, 0);

    /// <summary>
    /// The offset from the player's position for the bottom raycast.
    /// </summary>
    [SerializeField] private Vector3 bottomOffset = new Vector3(0, -1.5f, 0);

    /// <summary>
    /// The offset from the player's position for the top raycast.
    /// </summary>
    [SerializeField] private Vector3 topOffset = new Vector3(0, 0.5f, 0);

    #endregion

    #region Death Implementation

    /// <summary>
    /// Flag to determine if the essence can be spawned.
    /// </summary>
    public bool canSpawnEssence = true;

    /// <summary>
    /// Implements player-specific death behavior.
    /// </summary>
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

    #endregion

    #region Private Methods

    /// <summary>
    /// Sets up the essence prefab.
    /// </summary>
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

    /// <summary>
    /// Moves the player essence to a safe position.
    /// </summary>
    /// <param name="playerEssence">The player essence to move.</param>
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

    /// <summary>
    /// Saves the game after a frame.
    /// </summary>
    /// <param name="active_slot">The active save slot.</param>
    /// <returns>A coroutine.</returns>
    private IEnumerator SaveAfterFrame(int active_slot)
    {
        // Wait for a frame to ensure that any changes to the prefab's state are applied
        yield return null;

        // Save the game to the specified slot
        if (SaveSystem.HasSavedGameInSlot(active_slot))
            SaveSystem.SaveToSlot(active_slot);
    }

    #endregion
}
