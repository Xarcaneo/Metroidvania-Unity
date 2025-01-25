using UnityEngine;

/// <summary>
/// An effect that teleports the caster to a target position.
/// </summary>
[CreateAssetMenu(fileName = "TeleportEffect", menuName = "SpellBook/Effects/Teleport")]
public class TeleportEffect : SpellEffect
{
    public override void TriggerEffect(GameObject caster, GameObject target)
    {
        if (caster == null)
        {
            Debug.LogError("Caster is null. Cannot teleport.");
            return;
        }
        else
        {
            Debug.LogError("TELEPROT.");
        }
    }
}
