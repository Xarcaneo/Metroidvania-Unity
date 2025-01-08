using UnityEngine;

/// <summary>
/// Handles parallax scrolling effect for background layers.
/// Creates depth illusion by moving layers at different speeds relative to camera movement.
/// </summary>
/// <remarks>
/// This component should be attached to background sprite objects.
/// The parallax effect is calculated based on the camera's position and a custom effect multiplier.
/// 
/// Features:
/// - Infinite scrolling with sprite repetition
/// - Customizable parallax strength per layer
/// - Automatic length calculation based on sprite bounds
/// </remarks>
public class Paralax : MonoBehaviour
{
    #region Private Fields
    /// <summary>
    /// Length of the sprite for calculating repetition
    /// </summary>
    private float length;
    
    /// <summary>
    /// Initial X position of the sprite
    /// </summary>
    private float startpos;
    #endregion

    #region Public Fields
    /// <summary>
    /// Reference to the main camera GameObject
    /// </summary>
    public GameObject cam;

    /// <summary>
    /// Strength of the parallax effect (0 = moves with camera, 1 = stays still)
    /// </summary>
    [Range(0f, 1f)]
    [Tooltip("How much the layer moves relative to the camera (0 = moves with camera, 1 = stays still)")]
    public float paralaxEffect;
    #endregion

    /// <summary>
    /// Initializes the parallax component by storing initial position and calculating sprite length
    /// </summary>
    private void Start()
    {
        // Store initial position for reference
        startpos = transform.position.x;
        
        // Get sprite width for repetition calculations
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    /// <summary>
    /// Updates the parallax effect each physics frame
    /// </summary>
    /// <remarks>
    /// The parallax effect is achieved by:
    /// 1. Calculating relative movement based on camera position
    /// 2. Moving the sprite accordingly
    /// 3. Handling sprite repetition when camera moves beyond sprite bounds
    /// </remarks>
    private void FixedUpdate()
    {
        // Calculate relative positions
        float temp = (cam.transform.position.x * (1 - paralaxEffect));
        float dist = (cam.transform.position.x * paralaxEffect);

        // Update position with parallax offset
        transform.position = new Vector3(startpos + dist, transform.position.y, 0);

        // Handle sprite repetition
        if (temp > startpos + length) 
            startpos += length;
        else if (temp < startpos - length) 
            startpos -= length;
    }
}
