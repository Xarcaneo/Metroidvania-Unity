using UnityEngine;

public class TilemapDestruction : MonoBehaviour
{
    [SerializeField] private LayerMask damageSourceLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & damageSourceLayer) != 0)
        {
            Destroy(gameObject); // Destroy the Tilemap GameObject
        }
    }

  
}
