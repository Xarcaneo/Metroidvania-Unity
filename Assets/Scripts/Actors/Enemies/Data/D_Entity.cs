using UnityEngine;

/// <summary>
/// Base data container for enemy entity configuration.
/// Contains common settings shared by all enemy types.
/// </summary>
[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    [Header("Detection Settings")]
    [Tooltip("Layer mask for player detection")]
    public LayerMask whatIsPlayer;

    private void OnValidate()
    {
        if (whatIsPlayer.value == 0)
        {
            Debug.LogWarning($"Player layer mask not set in {name}");
        }
    }
}