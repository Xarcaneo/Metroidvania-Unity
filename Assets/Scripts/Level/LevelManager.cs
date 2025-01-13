using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level-specific functionality including player spawning and scene transitions.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Player m_playerPref;
    [SerializeField] private Vector3 m_defaultSpawnPosition = new Vector3(-10, -10, 0);

    [Header("Level Settings")]
    [SerializeField] private LocationName m_areaName;

    public LocationName AreaName => m_areaName;

    private void OnEnable()
    {
        SpawnPlayer();

        Menu.GameMenu.Instance.locationNameIndicator.Cancelcoroutine();

        if (m_areaName != LocationName.None)
            StartCoroutine(DelayedAreaChanged());
    }

    /// <summary>
    /// Spawns the player in the level, either at a designated spawn point or at the default position.
    /// Handles player orientation based on the entrance used to enter the level.
    /// </summary>
    private void SpawnPlayer()
    {
        // 1) Look for "SpawnPoint"
        GameObject spawnPointObj = GameObject.FindGameObjectWithTag("FallbackSpawnPoint");
        if (spawnPointObj != null)
        {
            // Spawn at m_defaultSpawnPosition
            var player = Instantiate(m_playerPref, m_defaultSpawnPosition, Quaternion.identity);

            // Set facing direction based on spawn point
            SetPlayerFacingDirection(player.gameObject, spawnPointObj);
            return;
        }

        // 2) If no "SpawnPoint," look for "DefaultSpawnPoint"
        spawnPointObj = GameObject.FindGameObjectWithTag("PrimarySpawnPoint");
        if (spawnPointObj != null)
        {
            // Spawn at the DefaultSpawnPoint's position
            var position = spawnPointObj.transform.position;
            Instantiate(m_playerPref, position, Quaternion.identity);
            return;
        }

        // 3) If neither point is found, use m_defaultSpawnPosition
        Instantiate(m_playerPref, m_defaultSpawnPosition, Quaternion.identity);
    }

    /// <summary>
    /// Sets the player's facing direction based on the spawn point's configuration.
    /// </summary>
    /// <param name="player">The spawned player instance</param>
    /// <param name="spawnPointObj">The spawn point object containing direction information</param>
    private void SetPlayerFacingDirection(GameObject player, GameObject spawnPointObj)
    {
        if (player == null) return;

        var spawnPoint = spawnPointObj.GetComponent<FallbackSpawnPoint>();
        if (spawnPoint == null) return;

        var playerComponent = player.GetComponent<Player>();
        if (playerComponent == null) return;

        var movement = playerComponent.Core.GetCoreComponent<Movement>();
        if (movement != null)
        {
            // shouldFaceLeft true -> face left (-1), false -> face right (1)
            int direction = spawnPoint.shouldFaceLeft ? -1 : 1;
            movement.Flip(direction);
        }

        // Clean up the spawn point immediately after using it
        Destroy(spawnPointObj);
    }

    /// <summary>
    /// Triggers the area changed event after a short delay.
    /// </summary>
    private IEnumerator DelayedAreaChanged()
    {
        yield return new WaitForSeconds(0.5f);
        GameEvents.Instance.AreaChanged(m_areaName.ToDisplayName());
    }
}
