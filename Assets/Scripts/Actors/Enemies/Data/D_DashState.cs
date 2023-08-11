using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDashStateData", menuName = "Data/State Data/Dash State")]
public class D_DashState : ScriptableObject
{
    [Header("Player detection variables")]
    public float detectionWidth;    // width of the detection area
    public float detectionHeight;   // height of the detection area
    public LayerMask entityLayer;   // layer(s) of the entity to detect
    public Vector3 raycastTopPosition;
    public Vector3 raycastBottomPosition;
    public int groundRaycastDistance = 1;

    [Header("Collision layers")]
    public LayerMask collisionLayer;   // layer(s) of the collision to detect

    [Header("Dash State variables")]
    public Vector2 dashVelocity = new Vector2(30, 0);
    public Vector2 dashAngle = new Vector2(1, 0);
    public float dashTime = 0.2f;
}
