using UnityEngine;

/// <summary>
/// Handles the behavior of horizontal entrances in the game.
/// Controls player input and orientation when entering through this entrance.
/// </summary>
public class HorizontalEntrance : Entrance
{
    [SerializeField] private GameObject spawnPointPrefab;

    /// <summary>
    /// Called when a player enters this entrance.
    /// Creates a spawn point and sets its facing direction based on the entrance's scale.
    /// </summary>
    public override void EntranceEntered()
    {
        base.EntranceEntered();

        InputManager.Instance.isInputActive = false;
        GameEvents.Instance.DeactivatePlayerInput(true);

        // Create spawn point at the entrance position
        GameObject spawnPoint = Instantiate(spawnPointPrefab, transform.position, Quaternion.identity);
        DontDestroyOnLoad(spawnPoint);
        spawnPoint.tag = "FallbackSpawnPoint";

        // Set spawn point direction based on entrance scale
        FallbackSpawnPoint spawnPointComponent = spawnPoint.GetComponent<FallbackSpawnPoint>();
        if (spawnPointComponent != null)
        {
            // If entering from left (scale.x < 0), face left in new scene
            // If entering from right (scale.x > 0), face right in new scene
            spawnPointComponent.shouldFaceLeft = transform.localScale.x < 0;
        }
    }
}
