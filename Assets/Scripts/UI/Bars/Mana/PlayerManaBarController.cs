using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManaBarController : ManaBarController
{
    private bool _isNewSession = false;

    private void OnPlayerSpawned()
    {
        stats = Player.Instance.Core.GetCoreComponent<Stats>();

        if (_isNewSession)
        {
            SetMaxMana(stats.GetMaxMana()); //
            mana = stats.GetMaxMana();
            _isNewSession = false;
        }

        stats.ManaUsed += UseMana; // Assuming a UseMana method exists
        stats.ManaRestored += RestoreMana;
    }

    public void Initialize()
    {
        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        GameEvents.Instance.onPlayerDied += OnPlayerDied;
        GameEvents.Instance.onNewSession += OnNewSession;
    }

    private void OnPlayerDied()
    {
        //stats.ManaUsed -= UseMana;
        _isNewSession = true;
    }

    private void OnNewSession() => _isNewSession = true;

    override public void OnDestroy()
    {
        base.OnDestroy();

        GameEvents.Instance.onPlayerDied -= OnPlayerDied;
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
        GameEvents.Instance.onNewSession -= OnNewSession;
    }
}
