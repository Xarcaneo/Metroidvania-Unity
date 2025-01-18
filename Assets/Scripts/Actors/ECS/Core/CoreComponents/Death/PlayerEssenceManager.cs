using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// Manages the spawning and handling of player essences when the player dies.
/// </summary>
public class PlayerEssenceManager : MonoBehaviour
{
    #region Essence Prefab
    [SerializeField] private PlayerEssence m_playerEssencePref;
    #endregion

    #region Raycast Settings
    /// <summary>
    /// The layer mask for solid objects.
    /// </summary>
    [SerializeField] private LayerMask solidLayer;

    /// <summary>
    /// The offset from the essence's position for the left raycast.
    /// </summary>
    [SerializeField] private Vector3 downOffsetLeft = new Vector3(-2, 0f, 0);

    /// <summary>
    /// The offset from the essence's position for the right raycast.
    /// </summary>
    [SerializeField] private Vector3 downOffsetRight = new Vector3(2, 0f, 0);

    /// <summary>
    /// The offset from the essence's position for the bottom raycast.
    /// </summary>
    [SerializeField] private Vector3 bottomOffset = new Vector3(0, -1.5f, 0);

    /// <summary>
    /// The offset from the essence's position for the top raycast.
    /// </summary>
    [SerializeField] private Vector3 topOffset = new Vector3(0, 0.5f, 0);
    #endregion

    private void OnEnable()
    {
        GameEvents.Instance.onPlayerEssenceSpawn += SpawnEssence;
        GameEvents.Instance.onEssenceCollected += CollectEssence;
    }

    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerEssenceSpawn -= SpawnEssence;
            GameEvents.Instance.onEssenceCollected -= CollectEssence;
        }
    }

    /// <summary>
    /// Spawns a player essence at the specified position with the given souls amount.
    /// </summary>
    /// <param name="position">Position to spawn the essence</param>
    /// <param name="soulsAmount">Amount of souls to store in the essence</param>
    private void SpawnEssence(Vector2 position, int soulsAmount)
    {
        PlayerEssence essence = Instantiate(m_playerEssencePref, new Vector3(position.x, position.y, 0), Quaternion.identity);
        
        // Get the active area from the AreaManager and set it as the parent
        AreaDetector activeArea = AreaManager.Instance?.ActiveArea;
        if (activeArea != null)
        {
            essence.transform.SetParent(activeArea.transform);
            GameEvents.Instance.RoomEssenceChanged(activeArea.roomNumber,true);
            DialogueLua.SetVariable($"RoomHasEssence.{activeArea.roomNumber}", true);
        }

        // Extract souls from the player and store them in the essence
        int extractedSouls = essence.ExtractSoulsOnDeath(soulsAmount);
        GameEvents.Instance.SoulsChanged(-extractedSouls);

        // Adjust essence position to avoid walls and gaps
        AdjustEssencePosition(essence);

        // Initially hide the renderers and disable interaction until fully set up
        Renderer[] renderers = essence.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
        essence.canInteract = false;
    }

    /// <summary>
    /// Handles the collection of a player essence and updates its state.
    /// </summary>
    private void CollectEssence(PlayerEssence essence)
    {
        // Get the active area and update DialogueLua
        AreaDetector activeArea = AreaManager.Instance?.ActiveArea;
        if (activeArea != null)
        {
            DialogueLua.SetVariable($"RoomHasEssence.{activeArea.roomNumber}", false);
            GameEvents.Instance.RoomEssenceChanged(activeArea.roomNumber, false);
        }
        Destroy(essence.gameObject);
    }

    /// <summary>
    /// Adjusts the essence position to avoid walls and gaps.
    /// </summary>
    private void AdjustEssencePosition(PlayerEssence essence)
    {
        // Check if the essence is close to a wall or edge
        RaycastHit2D hitLeft = Physics2D.Raycast(essence.transform.position + topOffset, Vector2.left, 1f, solidLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(essence.transform.position + topOffset, Vector2.right, 1f, solidLayer);
        RaycastHit2D hitLeftBottom = Physics2D.Raycast(essence.transform.position + bottomOffset, Vector2.left, 1f, solidLayer);
        RaycastHit2D hitRightBottom = Physics2D.Raycast(essence.transform.position + bottomOffset, Vector2.right, 1f, solidLayer);
        RaycastHit2D hitBottomLeft = Physics2D.Raycast(essence.transform.position + downOffsetLeft, Vector2.down, 3f, solidLayer);
        RaycastHit2D hitBottomRight = Physics2D.Raycast(essence.transform.position + downOffsetRight, Vector2.down, 3f, solidLayer);

        Vector3 adjustment = Vector3.zero;

        // Adjust for walls
        if (hitLeft.collider != null || hitLeftBottom.collider != null)
        {
            adjustment.x += 1f;
        }
        else if (hitRight.collider != null || hitRightBottom.collider != null)
        {
            adjustment.x -= 1f;
        }

        // Adjust for gaps
        if (hitBottomLeft.collider == null)
        {
            adjustment.x += 1f;
        }
        else if (hitBottomRight.collider == null)
        {
            adjustment.x -= 1f;
        }

        // Apply the adjustment if any
        if (adjustment != Vector3.zero)
        {
            essence.transform.position += adjustment;
        }
    }
}
