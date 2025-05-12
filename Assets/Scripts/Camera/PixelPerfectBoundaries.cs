using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Adds camera boundaries to Unity's built-in PixelPerfectCamera.
/// Visualizes boundaries in the editor and ensures the camera stays within them.
/// </summary>
public class PixelPerfectBoundaries : MonoBehaviour
{
    [Header("Boundary Settings")]
    [Tooltip("Enable left boundary")]
    public bool UseLeftBoundary = true;
    
    [Tooltip("Enable right boundary")]
    public bool UseRightBoundary = true;
    
    [Tooltip("Enable top boundary")]
    public bool UseTopBoundary = true;
    
    [Tooltip("Enable bottom boundary")]
    public bool UseBottomBoundary = true;
    
    [Tooltip("Left boundary world position")]
    public float LeftBoundary = -10f;
    
    [Tooltip("Right boundary world position")]
    public float RightBoundary = 10f;
    
    [Tooltip("Top boundary world position")]
    public float TopBoundary = 10f;
    
    [Tooltip("Bottom boundary world position")]
    public float BottomBoundary = -10f;
    
    [Header("Visualization")]
    [Tooltip("Color for boundary visualization in the editor")]
    public Color BoundaryColor = new Color(1f, 0.5f, 0.5f, 0.8f);
    
    [Tooltip("Show camera frustum in the editor")]
    public bool ShowCameraFrustum = true;
    
    [Tooltip("Color for camera frustum visualization")]
    public Color FrustumColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    
    [Header("Debug")]
    [Tooltip("Enable debug logs")]
    public bool DebugLogs = false;
    
    // References
    private PixelPerfectCamera _pixelPerfectCamera;
    private Camera _camera;
    
    // Tracking variables
    private int _lastPixelRatio = -1;
    private float _lastOrthoSize = -1f;
    private Vector2 _lastCameraPosition;
    
    private void Awake()
    {
        // Get required components
        _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        _camera = GetComponent<Camera>();
        
        // Validate required components
        if (_pixelPerfectCamera == null)
        {
            Debug.LogError("PixelPerfectBoundaries requires Unity's PixelPerfectCamera component");
            enabled = false;
            return;
        }
        
        if (_camera == null)
        {
            Debug.LogError("PixelPerfectBoundaries requires a Camera component");
            enabled = false;
            return;
        }
        
        // Store initial position
        _lastCameraPosition = new Vector2(_camera.transform.position.x, _camera.transform.position.y);
        
        if (DebugLogs)
            Debug.Log("[PixelPerfectBoundaries] Initialized successfully");
    }
    
    // Variables to track position stability
    private Vector3 _lastEnforcedPosition;
    private bool _isStabilizing = false;
    private float _positionStabilityThreshold = 0.01f;
    private int _stabilityCounter = 0;
    private int _requiredStableFrames = 3;
    
    private void LateUpdate()
    {
        bool shouldUpdatePosition = false;
        
        // Check if pixel perfect settings changed
        if (_pixelPerfectCamera.pixelRatio != _lastPixelRatio || 
            _pixelPerfectCamera.orthographicSize != _lastOrthoSize)
        {
            _lastPixelRatio = _pixelPerfectCamera.pixelRatio;
            _lastOrthoSize = _pixelPerfectCamera.orthographicSize;
            shouldUpdatePosition = true;
            _isStabilizing = true; // Start stabilization when settings change
            _stabilityCounter = 0;
            
            if (DebugLogs)
                Debug.Log($"[PixelPerfectBoundaries] PixelPerfectCamera settings changed: PixelRatio={_lastPixelRatio}, OrthoSize={_lastOrthoSize}");
        }
        
        // Check if camera position changed significantly
        Vector2 currentPos = new Vector2(_camera.transform.position.x, _camera.transform.position.y);
        float positionDelta = Vector2.Distance(currentPos, _lastCameraPosition);
        
        // Only consider it a position change if it's beyond our stability threshold
        // and we're not in stabilization mode
        if (positionDelta > _positionStabilityThreshold && !_isStabilizing)
        {
            _lastCameraPosition = currentPos;
            shouldUpdatePosition = true;
        }
        
        // Apply boundary enforcement if needed
        if (shouldUpdatePosition || _isStabilizing)
        {
            EnforceBoundaries();
            
            // Check if position has stabilized
            if (_isStabilizing)
            {
                Vector3 currentCamPos = _camera.transform.position;
                float stabilityDelta = Vector3.Distance(currentCamPos, _lastEnforcedPosition);
                
                if (stabilityDelta < _positionStabilityThreshold)
                {
                    _stabilityCounter++;
                    if (_stabilityCounter >= _requiredStableFrames)
                    {
                        _isStabilizing = false;
                        if (DebugLogs)
                            Debug.Log("[PixelPerfectBoundaries] Camera position has stabilized");
                    }
                }
                else
                {
                    _stabilityCounter = 0;
                }
                
                _lastEnforcedPosition = currentCamPos;
            }
        }
    }
    
    private void EnforceBoundaries()
    {
        if (_camera == null)
            return;
            
        // Get current camera position
        float currentPosX = _camera.transform.position.x;
        float currentPosY = _camera.transform.position.y;
        
        // Calculate half screen size in world units
        float halfScreenWidth = _camera.orthographicSize * _camera.aspect;
        float halfScreenHeight = _camera.orthographicSize;
        
        // Initialize new position with current position
        float newPosX = currentPosX;
        float newPosY = currentPosY;
        
        // Check and enforce horizontal boundaries
        if (UseLeftBoundary && currentPosX - halfScreenWidth < LeftBoundary)
        {
            newPosX = LeftBoundary + halfScreenWidth;
            if (DebugLogs && !_isStabilizing) // Only log when not stabilizing to reduce spam
                Debug.Log($"[PixelPerfectBoundaries] Enforcing left boundary: {LeftBoundary}");
        }
        
        if (UseRightBoundary && currentPosX + halfScreenWidth > RightBoundary)
        {
            newPosX = RightBoundary - halfScreenWidth;
            if (DebugLogs && !_isStabilizing)
                Debug.Log($"[PixelPerfectBoundaries] Enforcing right boundary: {RightBoundary}");
        }
        
        // Check and enforce vertical boundaries
        if (UseBottomBoundary && currentPosY - halfScreenHeight < BottomBoundary)
        {
            newPosY = BottomBoundary + halfScreenHeight;
            if (DebugLogs && !_isStabilizing)
                Debug.Log($"[PixelPerfectBoundaries] Enforcing bottom boundary: {BottomBoundary}");
        }
        
        if (UseTopBoundary && currentPosY + halfScreenHeight > TopBoundary)
        {
            newPosY = TopBoundary - halfScreenHeight;
            if (DebugLogs && !_isStabilizing)
                Debug.Log($"[PixelPerfectBoundaries] Enforcing top boundary: {TopBoundary}");
        }
        
        // Apply the corrected position if it changed significantly
        // Use a larger threshold during stabilization to prevent oscillation
        float threshold = _isStabilizing ? 0.01f : 0.001f;
        
        if (Mathf.Abs(newPosX - currentPosX) > threshold || Mathf.Abs(newPosY - currentPosY) > threshold)
        {
            // Create a new position vector with the corrected coordinates
            Vector3 newPosition = new Vector3(newPosX, newPosY, _camera.transform.position.z);
            
            // Apply the position change
            _camera.transform.position = newPosition;
            
            // Round position to pixel grid if needed
            if (_pixelPerfectCamera.gridSnapping != PixelPerfectCamera.GridSnapping.None)
            {
                Vector3 roundedPos = _pixelPerfectCamera.RoundToPixel(_camera.transform.position);
                
                // Only apply the rounded position if it doesn't violate boundaries
                // This prevents oscillation caused by rounding
                bool roundingViolatesBoundaries = false;
                
                if (UseLeftBoundary && roundedPos.x - halfScreenWidth < LeftBoundary - threshold)
                    roundingViolatesBoundaries = true;
                    
                if (UseRightBoundary && roundedPos.x + halfScreenWidth > RightBoundary + threshold)
                    roundingViolatesBoundaries = true;
                    
                if (UseBottomBoundary && roundedPos.y - halfScreenHeight < BottomBoundary - threshold)
                    roundingViolatesBoundaries = true;
                    
                if (UseTopBoundary && roundedPos.y + halfScreenHeight > TopBoundary + threshold)
                    roundingViolatesBoundaries = true;
                
                if (!roundingViolatesBoundaries)
                    _camera.transform.position = roundedPos;
            }
            
            // Update last camera position
            _lastCameraPosition = new Vector2(_camera.transform.position.x, _camera.transform.position.y);
            
            if (DebugLogs && !_isStabilizing)
                Debug.Log($"[PixelPerfectBoundaries] Adjusted camera position to: ({_camera.transform.position.x}, {_camera.transform.position.y})");
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw boundaries
        Handles.color = BoundaryColor;
        
        // Draw horizontal boundaries
        if (UseLeftBoundary)
            Handles.DrawLine(new Vector3(LeftBoundary, TopBoundary + 1, 0), new Vector3(LeftBoundary, BottomBoundary - 1, 0));
            
        if (UseRightBoundary)
            Handles.DrawLine(new Vector3(RightBoundary, TopBoundary + 1, 0), new Vector3(RightBoundary, BottomBoundary - 1, 0));
        
        // Draw vertical boundaries
        if (UseTopBoundary)
            Handles.DrawLine(new Vector3(LeftBoundary - 1, TopBoundary, 0), new Vector3(RightBoundary + 1, TopBoundary, 0));
            
        if (UseBottomBoundary)
            Handles.DrawLine(new Vector3(LeftBoundary - 1, BottomBoundary, 0), new Vector3(RightBoundary + 1, BottomBoundary, 0));
        
        // Draw camera frustum if requested
        if (ShowCameraFrustum && Camera.main != null)
        {
            Handles.color = FrustumColor;
            
            Camera cam = Camera.main;
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            
            Vector3 camPos = cam.transform.position;
            Vector3 topLeft = new Vector3(camPos.x - halfWidth, camPos.y + halfHeight, 0);
            Vector3 topRight = new Vector3(camPos.x + halfWidth, camPos.y + halfHeight, 0);
            Vector3 bottomLeft = new Vector3(camPos.x - halfWidth, camPos.y - halfHeight, 0);
            Vector3 bottomRight = new Vector3(camPos.x + halfWidth, camPos.y - halfHeight, 0);
            
            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PixelPerfectBoundaries))]
public class PixelPerfectBoundariesEditor : Editor
{
    private SerializedProperty useLeftBoundary;
    private SerializedProperty useRightBoundary;
    private SerializedProperty useTopBoundary;
    private SerializedProperty useBottomBoundary;
    private SerializedProperty leftBoundary;
    private SerializedProperty rightBoundary;
    private SerializedProperty topBoundary;
    private SerializedProperty bottomBoundary;
    private SerializedProperty boundaryColor;
    private SerializedProperty showCameraFrustum;
    private SerializedProperty frustumColor;
    private SerializedProperty debugLogs;
    
    private void OnEnable()
    {
        useLeftBoundary = serializedObject.FindProperty("UseLeftBoundary");
        useRightBoundary = serializedObject.FindProperty("UseRightBoundary");
        useTopBoundary = serializedObject.FindProperty("UseTopBoundary");
        useBottomBoundary = serializedObject.FindProperty("UseBottomBoundary");
        leftBoundary = serializedObject.FindProperty("LeftBoundary");
        rightBoundary = serializedObject.FindProperty("RightBoundary");
        topBoundary = serializedObject.FindProperty("TopBoundary");
        bottomBoundary = serializedObject.FindProperty("BottomBoundary");
        boundaryColor = serializedObject.FindProperty("BoundaryColor");
        showCameraFrustum = serializedObject.FindProperty("ShowCameraFrustum");
        frustumColor = serializedObject.FindProperty("FrustumColor");
        debugLogs = serializedObject.FindProperty("DebugLogs");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);
        
        // Horizontal boundaries
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(useLeftBoundary, new GUIContent("Use Left"));
        EditorGUILayout.PropertyField(useRightBoundary, new GUIContent("Use Right"));
        EditorGUILayout.EndHorizontal();
        
        if (useLeftBoundary.boolValue)
            EditorGUILayout.PropertyField(leftBoundary, new GUIContent("Left Boundary"));
            
        if (useRightBoundary.boolValue)
            EditorGUILayout.PropertyField(rightBoundary, new GUIContent("Right Boundary"));
        
        // Vertical boundaries
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(useTopBoundary, new GUIContent("Use Top"));
        EditorGUILayout.PropertyField(useBottomBoundary, new GUIContent("Use Bottom"));
        EditorGUILayout.EndHorizontal();
        
        if (useTopBoundary.boolValue)
            EditorGUILayout.PropertyField(topBoundary, new GUIContent("Top Boundary"));
            
        if (useBottomBoundary.boolValue)
            EditorGUILayout.PropertyField(bottomBoundary, new GUIContent("Bottom Boundary"));
        
        // Visualization settings
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Visualization", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(boundaryColor, new GUIContent("Boundary Color"));
        EditorGUILayout.PropertyField(showCameraFrustum, new GUIContent("Show Camera Frustum"));
        
        if (showCameraFrustum.boolValue)
            EditorGUILayout.PropertyField(frustumColor, new GUIContent("Frustum Color"));
        
        // Debug settings
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(debugLogs, new GUIContent("Enable Debug Logs"));
        
        // Validate boundaries
        if (useLeftBoundary.boolValue && useRightBoundary.boolValue && leftBoundary.floatValue >= rightBoundary.floatValue)
        {
            EditorGUILayout.HelpBox("Left boundary must be less than right boundary!", MessageType.Error);
        }
        
        if (useBottomBoundary.boolValue && useTopBoundary.boolValue && bottomBoundary.floatValue >= topBoundary.floatValue)
        {
            EditorGUILayout.HelpBox("Bottom boundary must be less than top boundary!", MessageType.Error);
        }
        
        // Add button to fit boundaries to current level
        EditorGUILayout.Space();
        if (GUILayout.Button("Set Boundaries from Scene View"))
        {
            // Get the current scene view
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                // Get the visible rect in the scene view
                Rect sceneRect = sceneView.position;
                Vector3 bottomLeft = sceneView.camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
                Vector3 topRight = sceneView.camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
                
                // Set the boundaries
                leftBoundary.floatValue = bottomLeft.x;
                rightBoundary.floatValue = topRight.x;
                bottomBoundary.floatValue = bottomLeft.y;
                topBoundary.floatValue = topRight.y;
                
                Debug.Log($"Set boundaries from scene view: Left={leftBoundary.floatValue}, Right={rightBoundary.floatValue}, Bottom={bottomBoundary.floatValue}, Top={topBoundary.floatValue}");
            }
            else
            {
                Debug.LogWarning("No active scene view found");
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
