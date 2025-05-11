using UnityEngine;

/// <summary>
/// Simple background follower script that makes background images follow the camera.
/// </summary>
public class Paralax : MonoBehaviour
{
    #region Public Fields
    /// <summary>
    /// Reference to the main camera (optional - will use Camera.main if not set)
    /// </summary>
    public Camera targetCamera;

    /// <summary>
    /// Strength of the parallax effect (0 = moves exactly with camera, 1 = stays completely still)
    /// </summary>
    [Range(0f, 1f)]
    [Tooltip("How much the layer moves relative to the camera (0 = moves with camera, 1 = stays still)")]
    public float parallaxEffect = 0f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Initial position of this object
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// Initial position of the camera
    /// </summary>
    private Vector3 cameraStartPosition;
    #endregion

    /// <summary>
    /// Initializes references and starting positions
    /// </summary>
    private void Awake()
    {
        // Store the starting position of this object
        startPosition = transform.position;

        // If no camera is assigned, try to find the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // Store the starting position of the camera
        if (targetCamera != null)
        {
            cameraStartPosition = targetCamera.transform.position;
        }
    }

    /// <summary>
    /// Called once per frame after all Update functions have been called
    /// </summary>
    private void LateUpdate()
    {
        // If no camera is found, try to find one
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera != null)
            {
                cameraStartPosition = targetCamera.transform.position;
            }
            return;
        }

        // Calculate camera movement
        Vector3 cameraDelta = targetCamera.transform.position - cameraStartPosition;

        // Calculate the new position based on the parallax effect
        // When parallaxEffect = 0, the object moves exactly with the camera
        // When parallaxEffect = 1, the object stays completely still
        Vector3 newPosition = startPosition + cameraDelta * (1 - parallaxEffect);

        // Keep the original z position
        newPosition.z = transform.position.z;

        // Update the position
        transform.position = newPosition;
    }
}
