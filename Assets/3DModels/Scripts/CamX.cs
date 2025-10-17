using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MarbleCameraX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody target;   // The marble to follow

    [Header("Position Follow Settings")]
    [SerializeField] private float followSmoothTime = 0.2f;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float maxFollowSpeed = 30f;
    private Vector3 positionVelocity;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.35f;
    [SerializeField] private float maxRotationSpeed = 90f;
    [SerializeField] private float velocityThreshold = 0.3f;
    [SerializeField] private float rotationDamping = 0.85f;
    private float yVelocity;
    private Vector3 lastForward = Vector3.forward;
    private float lastDesiredYAngle;

    [Header("Collision Impact Smoothing")]
    [SerializeField] private float impactSmoothTime = 0.5f;
    [SerializeField] private float maxImpactMagnitude = 10f;
    private float currentImpactInfluence = 0f;
    private float impactVelocity = 0f;
    private Vector3 lastMarbleVelocity;

    // Expose target for other components
    public Rigidbody Target => target;

    void Start()
    {
        if (target != null)
        {
            lastMarbleVelocity = target.linearVelocity;
            lastDesiredYAngle = transform.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (!GameState.InputEnabled || target == null)
            return;

        DetectMarbleCollisionImpact();
        FollowPosition();
        FollowRotation();
    }

    private void DetectMarbleCollisionImpact()
    {
        Vector3 currentVelocity = target.linearVelocity;
        Vector3 velocityChange = currentVelocity - lastMarbleVelocity;
        float impactMagnitude = velocityChange.magnitude;

        if (impactMagnitude > 0.8f)
        {
            float normalizedImpact = Mathf.Clamp01(impactMagnitude / maxImpactMagnitude);
            currentImpactInfluence = Mathf.Max(currentImpactInfluence, normalizedImpact);
        }

        currentImpactInfluence = Mathf.SmoothDamp(
            currentImpactInfluence,
            0f,
            ref impactVelocity,
            impactSmoothTime
        );

        lastMarbleVelocity = currentVelocity;
    }

    /// <summary>
    /// Resets camera smoothing and positions camera at a safe offset from the marble.
    /// Call this after teleporting or respawning the marble.
    /// </summary>
    public void ResetCameraStateAndPosition()
    {
        if (target != null)
        {
            transform.position = target.position + followOffset;
            positionVelocity = Vector3.zero;
            impactVelocity = 0f;
            currentImpactInfluence = 0f;
            lastMarbleVelocity = target.linearVelocity;
            lastForward = Vector3.forward;
            lastDesiredYAngle = transform.eulerAngles.y;
            yVelocity = 0f;
        }
    }

    private void FollowPosition()
    {
        Vector3 targetPosition = target.position + followOffset;

        float dynamicSmoothTime = followSmoothTime + (currentImpactInfluence * impactSmoothTime * 0.8f);

        Vector3 desiredPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            dynamicSmoothTime,
            maxFollowSpeed
        );

        // No collision logic, just set position
        transform.position = desiredPosition;
    }

    private void FollowRotation()
    {
        Vector3 flatForward = new Vector3(target.linearVelocity.x, 0f, target.linearVelocity.z);

        float dynamicRotationSmoothTime = rotationSmoothTime + (currentImpactInfluence * impactSmoothTime);

        if (flatForward.sqrMagnitude > velocityThreshold * velocityThreshold)
        {
            lastForward = flatForward.normalized;
        }

        float desiredYAngle = Quaternion.LookRotation(lastForward).eulerAngles.y;

        desiredYAngle = Mathf.LerpAngle(lastDesiredYAngle, desiredYAngle, rotationDamping);
        lastDesiredYAngle = desiredYAngle;

        float newYAngle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            desiredYAngle,
            ref yVelocity,
            dynamicRotationSmoothTime,
            maxRotationSpeed
        );

        transform.rotation = Quaternion.Euler(0f, newYAngle, 0f);
    }

    void OnDrawGizmosSelected()
    {
        // Only draw the follow offset sphere for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Vector3 targetPos = target.position + followOffset;
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}
