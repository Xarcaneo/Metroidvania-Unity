using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DestructionLevel
{
    public GameObject levelObject;
}

/// <summary>
/// Handles the destruction of a tilemap with multiple destruction levels.
/// </summary>
public class TilemapDestruction : MonoBehaviour
{
    /// <summary>
    /// Array of destruction levels with their corresponding GameObjects.
    /// </summary>
    [SerializeField]
    private DestructionLevel[] destructionLevels;

    /// <summary>
    /// Current destruction level index.
    /// </summary>
    [SerializeField]
    [Range(0, 2)] // Assuming 3 levels (0, 1, 2)
    private int currentLevel = 0;

    /// <summary>
    /// Specifies the layer(s) that can trigger the destruction of the tilemap.
    /// </summary>
    [SerializeField]
    private LayerMask damageSourceLayer;

    /// <summary>
    /// The hidden entrance GameObject to reveal when the tilemap is destroyed.
    /// </summary>
    [SerializeField]
    private GameObject hiddenEntrance;

    [SerializeField]
    private DestructionAnimationHandler animationHandler;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Delay the update to avoid SendMessage errors
        EditorApplication.delayCall += () =>
        {
            if (this == null) return; // Check if object still exists
            UpdateVisualOnly();
        };
    }
#endif

    private void Start()
    {
        UpdateVisualOnly();
    }

    private void UpdateVisualOnly()
    {
        if (destructionLevels == null || destructionLevels.Length == 0)
            return;

        // Clamp current level to valid range
        currentLevel = Mathf.Clamp(currentLevel, 0, destructionLevels.Length - 1);

        // Update visibility of destruction level objects
        for (int i = 0; i < destructionLevels.Length; i++)
        {
            if (destructionLevels[i].levelObject != null)
            {
                destructionLevels[i].levelObject.SetActive(i == currentLevel);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & damageSourceLayer) != 0)
        {
            HandleDamage();
        }
    }

    private void HandleDamage()
    {
        currentLevel++;

        // If we've exceeded the last level, start final destruction
        if (currentLevel >= destructionLevels.Length)
        {
            hiddenEntrance.gameObject.SetActive(true);

            // Play the final destruction animation
            if (animationHandler != null)
            {
                animationHandler.PlayFinalDestructionAnimation();
            }

            // Trigger the HiddenRoomRevealed event
            GameEvents.Instance.HiddenRoomRevealed();
        }
        else
        {
            // Play normal destruction animation
            if (animationHandler != null)
            {
                animationHandler.PlayDestructionAnimation();
            }
            UpdateVisualOnly();
        }
    }

    // Called by DestructionAnimationHandler when final animation is complete
    public void DestroyWall()
    {
        Destroy(gameObject);
    }
}
