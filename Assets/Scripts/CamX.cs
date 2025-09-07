using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MarbleCameraX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody target;   // The marble to follow

    [Header("Position Follow Settings")]
    [SerializeField] private float followSmoothTime = 0.2f; // Seconds to reach target
    private Vector3 positionVelocity;  // Internal velocity for SmoothDamp

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.2f; // Seconds to rotate
    [SerializeField] private float maxRotationSpeed = 180f;   // Degrees per second
    private float yVelocity;              // For SmoothDampAngle
    private Vector3 lastForward = Vector3.forward; // Last valid movement direction

    void LateUpdate()
    {
        // ⛔ Do nothing if the game is completed
        if (GameState.IsCompleted || target == null)
            return;

        FollowPosition();
        FollowRotation();
    }

    /// <summary>
    /// Smoothly follows the marble's position with inertia.
    /// </summary>
    private void FollowPosition()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.position,
            ref positionVelocity,
            followSmoothTime
        );
    }

    /// <summary>
    /// Smoothly rotates the camera to face the marble's movement direction.
    /// </summary>
    private void FollowRotation()
    {
        // Get horizontal movement direction
        Vector3 flatForward = new Vector3(target.linearVelocity.x, 0f, target.linearVelocity.z);

        // Update lastForward only if the ball is moving
        if (flatForward.sqrMagnitude > 0.01f)
        {
            lastForward = flatForward.normalized;
        }

        // Compute desired yaw
        float desiredYAngle = Quaternion.LookRotation(lastForward).eulerAngles.y;

        // Smoothly interpolate angle with max speed
        float newYAngle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            desiredYAngle,
            ref yVelocity,
            rotationSmoothTime,
            maxRotationSpeed
        );

        transform.rotation = Quaternion.Euler(0f, newYAngle, 0f);
    }
}
