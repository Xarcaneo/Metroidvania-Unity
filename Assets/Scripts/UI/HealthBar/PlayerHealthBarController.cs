using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBarController : HealthBarController
{ 
    private void OnPlayerSpawned()
    {
        stats = Player.Instance.Core.GetCoreComponent<Stats>();
        SetMaxHealth(stats.GetMaxHealth());
        health = stats.GetMaxHealth();

        stats.Damaged += TakeDamage;
    }

    public void Initialize() => GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;

    override public void OnDestroy()
    {
        base.OnDestroy();

        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
    }
}
