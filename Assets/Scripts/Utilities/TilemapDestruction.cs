using UnityEngine;

public class TilemapDestruction : MonoBehaviour
{
    [SerializeField] private LayerMask damageSourceLayer;

    [SerializeField] private GameObject hiddenEntrance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & damageSourceLayer) != 0)
        {
            hiddenEntrance.GetComponent<SpriteRenderer>().enabled = true;
            GameEvents.Instance.HiddenRoomRevealed();
            Destroy(gameObject); // Destroy the Tilemap GameObject
        }
    }

  
}
