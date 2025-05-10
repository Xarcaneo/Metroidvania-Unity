using PixelCrushers;
using UnityEngine;
using UnityCore.AudioManager;

/// <summary>
/// An effect that teleports the caster to a target position.
/// </summary>
[CreateAssetMenu(fileName = "TeleportEffect", menuName = "SpellBook/Effects/Teleport")]
public class TeleportEffect : SpellEffect
{
    public override void TriggerEffect(Entity caster, Entity target)
    {
        if (caster == null)
        {
            Debug.LogError("Caster is null. Cannot teleport.");
            return;
        }

        var positionSaver = caster.GetComponent<PlayerPositionSaver>();
        if (positionSaver == null)
        {
            Debug.LogError("PlayerPositionSaver component not found on caster.");
            return;
        }

        PositionSaver.PositionData positionData = positionSaver.GetPositionData();
        
        if (string.IsNullOrEmpty(positionData.checkpointSceneName))
        {
            Debug.LogError("No checkpoint scene name found in position data.");
            return;
        }

        // Play teleport sound effect
        AudioManager.instance.PlaySound(AudioEventId.Effect_Spell_Teleport);

        // Trigger the scene change with the checkpoint scene name
        GameEvents.Instance.TriggerSceneChange(positionData.checkpointSceneName, "CheckPoint_SpawnPoint");
    }
}
