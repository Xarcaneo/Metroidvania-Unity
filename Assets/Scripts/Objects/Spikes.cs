using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null) damageable.InstantKill();

        if (collision.CompareTag("Player"))
        {
            Player.Instance.Core.GetCoreComponent<PlayerDeath>().canSpawnEssence = false;
        }
    }
}
