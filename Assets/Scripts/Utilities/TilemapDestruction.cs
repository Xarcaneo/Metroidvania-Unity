using UnityEngine;

/// <summary>
/// Handles the destruction of a tilemap and reveals a hidden entrance when a collision with a damage source occurs.
/// </summary>
public class TilemapDestruction : MonoBehaviour
{
    /// <summary>
    /// Specifies the layer(s) that can trigger the destruction of the tilemap.
    /// </summary>
    [SerializeField]
    private LayerMask damageSourceLayer;

    /// <summary>
    /// The hidden entrance GameObject to reveal when the tilemap is destroyed.
    /// </summary>
    [SerializeField]
    private GameObject hiddenEntrance;

    // Cached reference to the SpriteRenderer component of the hidden entrance.
    private SpriteRenderer hiddenEntranceRenderer;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Caches the SpriteRenderer component to improve performance.
    /// </summary>
    private void Awake()
    {
        hiddenEntranceRenderer = hiddenEntrance.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Triggered when another Collider2D enters the tilemap's trigger collider.
    /// Checks if the collider belongs to a damage source layer and handles tilemap destruction.
    /// </summary>
    /// <param name="other">The Collider2D that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider's layer matches the damage source layer mask.
        if (((1 << other.gameObject.layer) & damageSourceLayer) != 0)
        {
            // Reveal the hidden entrance by enabling its SpriteRenderer.
            hiddenEntranceRenderer.enabled = true;

            // Trigger the HiddenRoomRevealed event.
            GameEvents.Instance.HiddenRoomRevealed();

            // Destroy this tilemap GameObject.
            Destroy(gameObject);
        }
    }
}
