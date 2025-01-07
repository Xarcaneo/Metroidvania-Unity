using UnityEngine;

/// <summary>
/// Manages enemy behavior based on camera visibility.
/// Controls sound muting when enemy is out of camera view.
/// </summary>
public class EnemyInCameraView : CoreComponent
{
    #region References
    [Header("Required References")]
    [SerializeField, Tooltip("Transform of the enemy being tracked")]
    private Transform enemyTransform;

    [SerializeField, Tooltip("Component for controlling animation events")]
    private PlayAnimationEvents m_PlayAnimationEvents;

    /// <summary>
    /// Reference to the main camera in the scene
    /// </summary>
    private Camera mainCamera;
    #endregion

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError($"[{gameObject.name}] No camera tagged 'MainCamera' found in the scene!");
            enabled = false;
            return;
        }

        if (enemyTransform == null)
        {
            Debug.LogError($"[{gameObject.name}] Enemy transform reference is missing!");
            enabled = false;
            return;
        }

        if (m_PlayAnimationEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] PlayAnimationEvents reference is missing - sound control will be disabled.");
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (m_PlayAnimationEvents == null) return;

        var renderer = enemyTransform.GetComponent<Renderer>();
        if (renderer == null) return;

        // Update sound muting based on camera visibility
        m_PlayAnimationEvents.muteSounds = !GeometryUtility.TestPlanesAABB(
            GeometryUtility.CalculateFrustumPlanes(mainCamera),
            renderer.bounds
        );
    }
}
