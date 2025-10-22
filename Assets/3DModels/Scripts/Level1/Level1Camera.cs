using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Level1Camera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody target;   // The marble to follow

    [Header("Position Follow Settings")]
    [SerializeField] private float followDistance = 6.0f; // Fixed distance from target
    [SerializeField] private float verticalOffset = 2.0f; // Vertical offset from target's center
    [SerializeField] private float followSmoothTime = 0.2f;
    [SerializeField] private float maxFollowSpeed = 30f;
    private Vector3 positionVelocity;

    [Header("Centering Limiter")]
    [Tooltip("Maximum allowed distance (in units) the camera can lag behind the target's ideal position before it snaps to re-center.")]
    [SerializeField] private float centeringLimit = 1.0f; // Centering limit
    private Vector3 targetPosition; // The ideal position

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.35f;
    [SerializeField] private float maxRotationSpeed = 90f;
    [SerializeField] private float velocityThreshold = 0.3f;
    [SerializeField] private float rotationDamping = 0.85f;
    private float yVelocity;
    private Vector3 lastForward = Vector3.forward;
    private float lastDesiredYAngle;
    private Quaternion yRotationOnly; // Stores the Y-rotation only, used for marble input and positioning

    [Header("Impact Settings (Position/Rotation Smoothing)")]
    [SerializeField] private float impactSmoothTime = 0.5f;
    [SerializeField] private float maxImpactMagnitude = 10f;
    private float currentImpactInfluence = 0f;
    private float impactVelocity = 0f;
    private Vector3 lastMarbleVelocity;

    [Header("Tilt Settings (Local X/Z Rotation)")]
    [SerializeField] private float velocityTiltAngle = 32f;
    [SerializeField] private float accelerationTiltAngle = 24f;
    [SerializeField] private float tiltSmoothTime = 0.4f;
    [SerializeField] private float speedBasedTiltMultiplier = 1.6f;
    [SerializeField] private float maxSpeedForTilt = 20f;
    [SerializeField] private float tiltDamping = 0.8f;

    [Header("Impact Settings (Tilt Reduction)")]
    [SerializeField] private float impactTiltReduction = 0.3f;
    [SerializeField] private float impactDetectionThreshold = 1.5f;
    private Vector3 lastVelocity;
    private Vector3 tiltVelocity;
    private Vector3 currentTilt;
    private Vector3 lastTargetTilt;
    private float impactInfluence = 0f; // For tilt reduction
    private float impactInfluenceVelocity = 0f;

    // Public properties for Marble script to use for flat, camera-relative movement
    public Vector3 CameraForwardFlat => yRotationOnly * Vector3.forward;
    public Vector3 CameraRightFlat => yRotationOnly * Vector3.right;
    public Rigidbody Target => target;

    void Start()
    {
        if (target != null)
        {
            lastMarbleVelocity = target.linearVelocity;
            lastVelocity = target.linearVelocity;
            lastDesiredYAngle = transform.eulerAngles.y;

            // Initial position reset
            yRotationOnly = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            targetPosition = target.position
                             + (yRotationOnly * Vector3.back * followDistance)
                             + (yRotationOnly * Vector3.up * verticalOffset);
            transform.position = targetPosition;
        }
    }

    void LateUpdate()
    {
        if (!GameState.InputEnabled || target == null)
        {
            // If input is disabled, smoothly move to zero tilt and stop following
            currentTilt = Vector3.SmoothDamp(currentTilt, Vector3.zero, ref tiltVelocity, tiltSmoothTime);
            transform.localRotation = Quaternion.Euler(currentTilt);
            return;
        }

        DetectCollisionImpact(); // Detect impact for tilt reduction
        DetectMarbleCollisionImpact(); // Detect impact for position/rotation smoothing

        FollowRotation();
        FollowPosition();

        CalculatePhysicsBasedTilt();
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

    private void FollowPosition()
    {
        // 1. Calculate the ideal position (targetPosition) based on current Y rotation
        targetPosition = target.position
                         + (yRotationOnly * Vector3.back * followDistance)
                         + (yRotationOnly * Vector3.up * verticalOffset);

        // 2. Centering Limiter Check
        if (Vector3.Distance(transform.position, targetPosition) > centeringLimit)
        {
            // Snap to target position if the camera is too far off-center
            transform.position = targetPosition;
            positionVelocity = Vector3.zero;
            return;
        }

        // 3. SmoothDamp logic
        float dynamicSmoothTime = followSmoothTime + (currentImpactInfluence * impactSmoothTime * 0.8f);

        Vector3 desiredPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            dynamicSmoothTime,
            maxFollowSpeed
        );

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

        // Damping before smoothing
        desiredYAngle = Mathf.LerpAngle(lastDesiredYAngle, desiredYAngle, rotationDamping);
        lastDesiredYAngle = desiredYAngle;

        float newYAngle = Mathf.SmoothDampAngle(
            yRotationOnly.eulerAngles.y,
            desiredYAngle,
            ref yVelocity,
            dynamicRotationSmoothTime,
            maxRotationSpeed
        );

        yRotationOnly = Quaternion.Euler(0f, newYAngle, 0f);
    }

    private void DetectCollisionImpact()
    {
        Vector3 currentVelocity = target.linearVelocity;
        Vector3 velocityChange = currentVelocity - lastVelocity;
        float impactMagnitude = velocityChange.magnitude;

        float impactDirection = Vector3.Dot(velocityChange, lastVelocity.normalized);
        bool isImpact = impactDirection < 0;

        if (isImpact && impactMagnitude > impactDetectionThreshold)
        {
            impactInfluence = Mathf.Max(impactInfluence, Mathf.Clamp01(impactMagnitude / 8f));
        }

        impactInfluence = Mathf.SmoothDamp(impactInfluence, 0f, ref impactInfluenceVelocity, 0.6f);
    }

    private void CalculatePhysicsBasedTilt()
    {
        Vector3 velocity = target.linearVelocity;
        Vector3 acceleration = (velocity - lastVelocity) / Time.deltaTime;
        lastVelocity = velocity;

        if (velocity.magnitude < velocityThreshold)
        {
            currentTilt = Vector3.SmoothDamp(currentTilt, Vector3.zero, ref tiltVelocity, tiltSmoothTime);
            transform.rotation = yRotationOnly * Quaternion.Euler(currentTilt);
            return;
        }

        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 flatAcceleration = new Vector3(acceleration.x, 0f, acceleration.z);

        float speedMultiplier = 1f + (Mathf.Clamp01(flatVelocity.magnitude / maxSpeedForTilt) * speedBasedTiltMultiplier);

        // Banking tilt (Z-axis)
        float velocityTiltZ = 0f;
        if (flatVelocity.magnitude > velocityThreshold)
        {
            Vector3 right = Vector3.Cross(Vector3.up, flatVelocity.normalized);
            float lateralAccel = Vector3.Dot(flatAcceleration, right);
            velocityTiltZ = Mathf.Clamp(lateralAccel * velocityTiltAngle * speedMultiplier,
                                       -velocityTiltAngle * speedMultiplier,
                                       velocityTiltAngle * speedMultiplier);
        }

        // Acceleration tilt (X-axis)
        float accelerationTiltX = 0f;
        if (flatVelocity.magnitude > velocityThreshold)
        {
            float forwardAccel = Vector3.Dot(flatAcceleration, flatVelocity.normalized);
            accelerationTiltX = Mathf.Clamp(-forwardAccel * accelerationTiltAngle * speedMultiplier,
                                           -accelerationTiltAngle * speedMultiplier,
                                           accelerationTiltAngle * speedMultiplier);
        }

        Vector3 targetTilt = new Vector3(accelerationTiltX, 0f, velocityTiltZ);

        // Falling Downward Camera Tilt
        float fallingTiltX = 0f;
        if (velocity.y < -1f)
        {
            fallingTiltX = Mathf.Clamp(Mathf.Abs(velocity.y) * 6f, 0f, 90f);
            targetTilt.x += fallingTiltX;
        }

        // Apply impact reduction
        float tiltMultiplier = 1f - (impactInfluence * impactTiltReduction);
        targetTilt *= tiltMultiplier;

        // Add damping for smoother transitions
        targetTilt = Vector3.Lerp(lastTargetTilt, targetTilt, tiltDamping);
        lastTargetTilt = targetTilt;

        // Use dynamic smooth time - longer during impacts and generally smoother
        float dynamicSmoothTime = tiltSmoothTime + (impactInfluence * 0.4f);
        currentTilt = Vector3.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, dynamicSmoothTime);

        // Combine Y rotation and X/Z tilt
        transform.rotation = yRotationOnly * Quaternion.Euler(currentTilt);
    }

    /// <summary>
    /// Resets camera smoothing and positions camera at a safe offset from the marble.
    /// Call this after teleporting or respawning the marble.
    /// </summary>
    public void ResetCameraStateAndPosition()
    {
        if (target != null)
        {
            yRotationOnly = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            targetPosition = target.position
                             + (yRotationOnly * Vector3.back * followDistance)
                             + (yRotationOnly * Vector3.up * verticalOffset);

            transform.position = targetPosition;
            positionVelocity = Vector3.zero;
            impactVelocity = 0f;
            currentImpactInfluence = 0f;
            lastMarbleVelocity = target.linearVelocity;
            lastVelocity = target.linearVelocity;
            lastForward = Vector3.forward;
            lastDesiredYAngle = transform.eulerAngles.y;
            yVelocity = 0f;
            currentTilt = Vector3.zero;
        }
    }
}