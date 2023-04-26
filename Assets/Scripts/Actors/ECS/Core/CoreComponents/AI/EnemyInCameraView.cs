using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInCameraView : CoreComponent
{
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private PlayAnimationEvents m_PlayAnimationEvents;

    // Reference to the camera
    private Camera mainCamera;
    
    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;

        // Check if the main camera was found
        if (mainCamera == null)
        {
            Debug.LogError("No camera tagged 'MainCamera' found in the scene!");
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if the enemy's bounding box is within the camera's frustum planes
        if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(mainCamera), enemyTransform.GetComponent<Renderer>().bounds))
        {
            // Enemy is in camera view
            m_PlayAnimationEvents.muteSounds = false;
        }
        else
        {
            // Enemy is not in camera view
            m_PlayAnimationEvents.muteSounds = true;
        }
    }
}
