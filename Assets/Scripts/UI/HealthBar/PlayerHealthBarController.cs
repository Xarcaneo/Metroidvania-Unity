using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBarController : HealthBarController
{
    private bool _isNewSession = false;

    private void OnPlayerSpawned()
    {
        stats = Player.Instance.Core.GetCoreComponent<Stats>();

        if (_isNewSession)
        {
            SetMaxHealth(stats.GetMaxHealth());
            health = stats.GetMaxHealth();
            _isNewSession = false;
        }

        stats.Damaged += TakeDamage;
    }

    public void Initialize()
    {
        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        GameEvents.Instance.onPlayerDied += OnPlayerDied;
        GameEvents.Instance.onNewSession += OnNewSesion;
    }

    private void OnPlayerDied()
    {
        stats.Damaged -= TakeDamage;
        _isNewSession = true;
    }

    private void OnNewSesion() => _isNewSession = true;

    override public void OnDestroy()
    {
        base.OnDestroy();

        GameEvents.Instance.onPlayerDied -= OnPlayerDied;
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
        GameEvents.Instance.onNewSession -= OnNewSesion;
    }
}
