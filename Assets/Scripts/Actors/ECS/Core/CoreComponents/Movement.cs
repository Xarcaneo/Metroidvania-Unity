using System;
using UnityEngine;

/// <summary>
/// Handles entity movement, including velocity control, facing direction, and physics interactions.
/// </summary>
public class Movement : CoreComponent
{
    #region Events

    /// <summary>
    /// Event triggered when the last safe position is updated.
    /// </summary>
    public event Action<Vector2> OnLastSafePositionUpdated;

    #endregion

    #region Components

    /// <summary>
    /// Reference to the entity's Rigidbody2D component.
    /// </summary>
    private Rigidbody2D rb;
    public Rigidbody2D RB => rb;

    #endregion

    #region Movement Properties

    /// <summary>
    /// Current facing direction of the entity (1 for right, -1 for left).
    /// </summary>
    private int facingDirection = 1;
    public int FacingDirection => facingDirection;

    /// <summary>
    /// The last safe position of the entity (not on hazards).
    /// </summary>
    public Vector2 LastSafePosition { get; private set; }

    /// <summary>
    /// Whether the entity can change its velocity.
    /// </summary>
    public bool CanSetVelocity { get; set; } = true;

    /// <summary>
    /// Current velocity of the entity.
    /// </summary>
    private Vector2 workspace;
    public Vector2 CurrentVelocity { get; private set; }

    #endregion

    #region Movement Settings

    /// <summary>
    /// Maximum slope angle the entity can traverse.
    /// </summary>
    [SerializeField] public float maxSlopeAngle = 45;

    #endregion

    #region Dependencies

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes required components and default values.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInParent<Rigidbody2D>();
        facingDirection = 1;
        CanSetVelocity = true;
    }

    /// <summary>
    /// Updates the current velocity from the Rigidbody2D.
    /// </summary>
    public override void LogicUpdate()
    {
        CurrentVelocity = rb.velocity;
        
        if (CollisionSenses.Ground)
        {
            // Update last safe position only when grounded and not on hazards
            if (!CollisionSenses.IsOnHazard)
            {
                LastSafePosition = transform.position;

                // Trigger the event when LastSafePosition changes
                OnLastSafePositionUpdated?.Invoke(LastSafePosition);
            }
        }
    }

    #endregion

    #region Movement Control

    /// <summary>
    /// Sets velocity using a single velocity value and direction angle.
    /// </summary>
    /// <param name="velocity">The velocity magnitude</param>
    /// <param name="angle">The direction angle as a Vector2</param>
    /// <param name="direction">The facing direction (1 or -1)</param>
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        SetFinalVelocity();
    }

    /// <summary>
    /// Sets velocity using separate x and y velocities and direction angle.
    /// </summary>
    /// <param name="velocity">The velocity vector</param>
    /// <param name="angle">The direction angle as a Vector2</param>
    /// <param name="direction">The facing direction (1 or -1)</param>
    public void SetVelocity(Vector2 velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity.x * direction, angle.y * velocity.y);
        SetFinalVelocity();
    }

    /// <summary>
    /// Sets the x component of the velocity.
    /// </summary>
    /// <param name="velocity">The new x velocity</param>
    public void SetVelocityX(float velocity)
    {
        if (CollisionSenses && CollisionSenses.Ground && CollisionSenses.slopeDownAngle <= maxSlopeAngle)
        {
            // When on a slope, use slope-adjusted movement
            float slopeModifier = 1f - (CollisionSenses.slopeDownAngle / 90f);
            workspace.Set(
                velocity * slopeModifier,
                CurrentVelocity.y
            );
        }
        else
        {
            workspace.Set(velocity, CurrentVelocity.y);
        }
        SetFinalVelocity();
    }

    /// <summary>
    /// Sets the x component of the velocity while on a slope.
    /// </summary>
    /// <param name="velocity">The new x velocity</param>
    public void SetVelocityXOnSlope(float velocity)
    {
        // Calculate slope movement with a slight downward bias to prevent "launching"
        float slopeDownwardBias = 0.1f;
        workspace.Set(
            velocity * CollisionSenses.slopeNormalPerp.x,
            (velocity * CollisionSenses.slopeNormalPerp.y) - slopeDownwardBias
        );
        SetFinalVelocity();
    }

    /// <summary>
    /// Sets the y component of the velocity.
    /// </summary>
    /// <param name="velocity">The new y velocity</param>
    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        SetFinalVelocity();
    }

    /// <summary>
    /// Sets the velocity to zero.
    /// </summary>
    public void SetVelocityZero()
    {
        workspace = Vector2.zero;
        SetFinalVelocity();
    }

    /// <summary>
    /// Checks if the entity should flip based on the input direction.
    /// </summary>
    /// <param name="xInput">The input direction</param>
    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
        {
            Flip();
        }
    }

    /// <summary>
    /// Flips the entity's facing direction.
    /// </summary>
    public void Flip()
    {
        facingDirection *= -1;
        rb.transform.localScale = new Vector3(rb.transform.localScale.x * -1, rb.transform.localScale.y, rb.transform.localScale.z);
    }

    /// <summary>
    /// Sets the entity's facing direction.
    /// </summary>
    /// <param name="direction">The new facing direction (1 or -1)</param>
    public void Flip(int direction)
    {
        if (direction != 0)
        {
            facingDirection = direction;
            rb.transform.localScale = new Vector3(Mathf.Abs(rb.transform.localScale.x) * direction, rb.transform.localScale.y, rb.transform.localScale.z);
        }
    }

    /// <summary>
    /// Checks if the entity can climb a slope at its current position.
    /// </summary>
    /// <returns>True if the slope is climbable, false otherwise</returns>
    public bool CanClimbSlope()
    {
        if (!CollisionSenses.Ground)
            return true;

        return CollisionSenses.slopeDownAngle <= maxSlopeAngle;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Applies the calculated velocity to the Rigidbody2D if allowed.
    /// </summary>
    private void SetFinalVelocity()
    {
        if (CanSetVelocity)
        {
            rb.velocity = workspace;
            CurrentVelocity = workspace;
        }
    }

    #endregion
}
