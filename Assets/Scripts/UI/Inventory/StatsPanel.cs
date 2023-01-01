using Opsive.UltimateInventorySystem.UI.Panels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    private Stats stats;

    [SerializeField] TextMeshProUGUI AttackText;
    [SerializeField] TextMeshProUGUI DefenseText;
    [SerializeField] TextMeshProUGUI HealthText;

    private void OnPlayerSpawned()
    {
        stats = Player.Instance.Core.GetCoreComponent<Stats>();
        stats.StatsUpdated += Draw;
        Draw();
    }

    private void OnDestroy()
    {
        stats.StatsUpdated -= Draw;
    }

    public void Initialize() => GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;

    private void Draw()
    {
        AttackText.text = stats.GetAttack().ToString();
        DefenseText.text = stats.GetDefense().ToString();
        HealthText.text = stats.GetMaxHealth().ToString();
    }
}
