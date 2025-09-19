using UnityEngine;

[RequireComponent(typeof(Transform))]
public class MarbleCameraX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody target;   // The marble to follow

    [Header("Position Follow Settings")]
    [SerializeField] private float followSmoothTime = 0.2f;      // Increased from 0.15f for smoother following
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float maxFollowSpeed = 30f;         // Limit max follow speed for smoother movement
    private Vector3 positionVelocity;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.35f;   // Increased from 0.25f for smoother rotation
    [SerializeField] private float maxRotationSpeed = 90f;      // Reduced from 120f for smoother rotation
    [SerializeField] private float velocityThreshold = 0.3f;     // Lowered from 0.5f for more responsive rotation
    [SerializeField] private float rotationDamping = 0.85f;     // Add damping for extra smoothness
    private float yVelocity;
    private Vector3 lastForward = Vector3.forward;
    private float lastDesiredYAngle;

    [Header("Camera Collision Settings")]
    [SerializeField] private LayerMask wallLayerMask = -1;
    [SerializeField] private float cameraRadius = 0.5f;
    [SerializeField] private float minDistanceFromWall = 0.2f;
    [SerializeField] private float collisionSmoothTime = 0.15f;  // Slightly increased for smoother collision response
    [SerializeField] private bool enableCollisionDebug = false;
    private Vector3 collisionVelocity;

    [Header("Collision Impact Smoothing")]
    [SerializeField] private float impactSmoothTime = 0.5f;      // Increased from 0.4f for smoother impact handling
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

        if (impactMagnitude > 0.8f)  // Lowered threshold for better detection
        {
            float normalizedImpact = Mathf.Clamp01(impactMagnitude / maxImpactMagnitude);
            currentImpactInfluence = Mathf.Max(currentImpactInfluence, normalizedImpact);
        }

        // Smoother impact influence decay
        currentImpactInfluence = Mathf.SmoothDamp(
            currentImpactInfluence,
            0f,
            ref impactVelocity,
            impactSmoothTime
        );

        lastMarbleVelocity = currentVelocity;
    }

    private void FollowPosition()
    {
        Vector3 targetPosition = target.position + followOffset;

        // Enhanced smoothing during impacts
        float dynamicSmoothTime = followSmoothTime + (currentImpactInfluence * impactSmoothTime * 0.8f);

        Vector3 desiredPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            dynamicSmoothTime,
            maxFollowSpeed  // Limit max speed for smoother movement
        );

        // Apply collision detection
        Vector3 finalPosition = CheckCameraCollision(desiredPosition);
        transform.position = finalPosition;
    }

    private Vector3 CheckCameraCollision(Vector3 desiredPosition)
    {
        Vector3 currentPos = transform.position;

        // First check: Is the desired position already inside a collider?
        Collider[] overlapping = Physics.OverlapSphere(desiredPosition, cameraRadius, wallLayerMask);
        if (overlapping.Length > 0)
        {
            if (enableCollisionDebug)
                Debug.Log($"Camera overlapping with {overlapping[0].name}");

            Vector3 pushDirection = Vector3.zero;

            foreach (var collider in overlapping)
            {
                Vector3 closestPoint = collider.ClosestPoint(desiredPosition);
                Vector3 pushDir = (desiredPosition - closestPoint).normalized;
                if (pushDir.magnitude > 0.1f)
                {
                    pushDirection += pushDir;
                }
            }

            if (pushDirection.magnitude > 0.1f)
            {
                pushDirection = pushDirection.normalized;
                Vector3 safePosition = desiredPosition + pushDirection * (cameraRadius + minDistanceFromWall);
                return Vector3.SmoothDamp(currentPos, safePosition, ref collisionVelocity, collisionSmoothTime);
            }
        }

        // Second check: Raycast from current to desired position
        Vector3 direction = desiredPosition - currentPos;
        float distance = direction.magnitude;

        if (distance > 0.001f)
        {
            RaycastHit hit;
            if (Physics.SphereCast(currentPos, cameraRadius * 0.9f, direction.normalized, out hit, distance, wallLayerMask))
            {
                if (enableCollisionDebug)
                    Debug.Log($"Camera would hit {hit.collider.name} at distance {hit.distance}");

                float safeDistance = Mathf.Max(0, hit.distance - (cameraRadius + minDistanceFromWall));
                Vector3 safePosition = currentPos + direction.normalized * safeDistance;

                return Vector3.SmoothDamp(currentPos, safePosition, ref collisionVelocity, collisionSmoothTime);
            }
        }

        // Third check: Final validation
        Collider[] finalCheck = Physics.OverlapSphere(desiredPosition, cameraRadius * 0.8f, wallLayerMask);
        if (finalCheck.Length > 0)
        {
            return Vector3.SmoothDamp(currentPos, currentPos, ref collisionVelocity, collisionSmoothTime);
        }

        return desiredPosition;
    }

    private void FollowRotation()
    {
        Vector3 flatForward = new Vector3(target.linearVelocity.x, 0f, target.linearVelocity.z);

        // Enhanced smoothing during impacts
        float dynamicRotationSmoothTime = rotationSmoothTime + (currentImpactInfluence * impactSmoothTime);

        if (flatForward.sqrMagnitude > velocityThreshold * velocityThreshold)
        {
            lastForward = flatForward.normalized;
        }

        float desiredYAngle = Quaternion.LookRotation(lastForward).eulerAngles.y;

        // Add damping to reduce sudden angle changes
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cameraRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cameraRadius + minDistanceFromWall);

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Vector3 targetPos = target.position + followOffset;
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}

