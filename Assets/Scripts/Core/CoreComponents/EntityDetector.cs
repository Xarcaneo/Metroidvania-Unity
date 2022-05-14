using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EntityDetector : CoreComponent
{
    [field: SerializeField]
    public bool EntityDetected { get; private set; }

    public Vector2 DirectionToTarget => target.transform.position - detectorOrigin.position;

    [Header("OverlapBoxParameters")]
    [SerializeField]
    private Transform detectorOrigin;
    public Vector2 detectorSize = Vector2.one;
    public Vector2 detectorOriginOffset = Vector2.zero;

    public float detectionDelay = 0.3f;

    public LayerMask detectorLayerMask;

    [Header("Gizmo parameters")]
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;

    private GameObject target;

    public GameObject Target
    {
        get => target;
        private set
        {
            target = value;
            EntityDetected = target != null;
        }
    }

    private void Start()
    {
        StartCoroutine(DetectionCoroutine());
    }

    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    public void PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize, 0, detectorLayerMask);

        if (collider != null)
        {
            Target = collider.gameObject;
        }
        else
        {

            Target = null;
        }
    }

    public bool GetEntityDetected()
    {
        return EntityDetected;
    }

    public int CheckFlipDirectionTowardsEntity()
    {
        if (target != null)
        {
            if (target.transform.position.x > detectorOrigin.transform.position.x)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (EntityDetected)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawCube((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }
}