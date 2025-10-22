using UnityEngine;

public class MarbleCameraY : MonoBehaviour
{
    [Header("Tilt Settings")]
    [SerializeField] private float velocityTiltAngle = 32f;      // Increased from 15f
    [SerializeField] private float accelerationTiltAngle = 24f;  // Increased from 10f
    [SerializeField] private float tiltSmoothTime = 0.4f;        // Increased from 0.3f for smoother movement
    [SerializeField] private float velocityThreshold = 0.3f;     // Lowered threshold for more responsive tilt

    [Header("Enhanced Tilt Settings")]
    [SerializeField] private float speedBasedTiltMultiplier = 1.6f;  // Extra tilt based on speed
    [SerializeField] private float maxSpeedForTilt = 20f;            // Max speed to consider for tilt
    [SerializeField] private float tiltDamping = 0.8f;              // Reduces sudden tilt changes

    [Header("X Tilt Limit (Up/Down)")]
    [SerializeField] private float maxXTilt = 30f; // Maximum allowed X tilt in degrees (up/down)

    [Header("Collision Impact Settings")]
    [SerializeField] private float impactTiltReduction = 0.3f;    // Reduced from 0.5f for more dramatic tilt
    [SerializeField] private float impactDetectionThreshold = 1.5f; // Lowered threshold for better detection

    [Header("References")]
    [SerializeField] private Rigidbody target;

    private Vector3 lastVelocity;
    private Vector3 tiltVelocity;
    private Vector3 currentTilt;
    private Vector3 lastTargetTilt;  // For extra smoothing
    private float impactInfluence = 0f;
    private float impactInfluenceVelocity = 0f;

    void Start()
    {
        if (target == null)
        {
            GameObject marble = GameObject.FindWithTag("Player");
            if (marble != null)
            {
                target = marble.GetComponent<Rigidbody>();
            }
        }

        if (target != null)
        {
            lastVelocity = target.linearVelocity;
        }
    }

    void LateUpdate()
    {
        if (!GameState.InputEnabled || target == null)
        {
            currentTilt = Vector3.SmoothDamp(currentTilt, Vector3.zero, ref tiltVelocity, tiltSmoothTime);
            // Clamp X tilt (up/down)
            currentTilt.x = Mathf.Clamp(currentTilt.x, -maxXTilt, maxXTilt);
            transform.localRotation = Quaternion.Euler(currentTilt);
            return;
        }

        DetectCollisionImpact();
        CalculatePhysicsBasedTilt();
    }

    private void DetectCollisionImpact()
    {
        Vector3 currentVelocity = target.linearVelocity;
        Vector3 velocityChange = currentVelocity - lastVelocity;
        float impactMagnitude = velocityChange.magnitude;

        // Only consider impact if velocity change is against previous movement
        float impactDirection = Vector3.Dot(velocityChange, lastVelocity.normalized);
        bool isImpact = impactDirection < 0; // Negative means change is against previous direction

        if (isImpact && impactMagnitude > impactDetectionThreshold)
        {
            impactInfluence = Mathf.Max(impactInfluence, Mathf.Clamp01(impactMagnitude / 8f));
        }

        // Smoother impact influence decay
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
            // Clamp X tilt (up/down)
            currentTilt.x = Mathf.Clamp(currentTilt.x, -maxXTilt, maxXTilt);
            transform.localRotation = Quaternion.Euler(currentTilt);
            return;
        }

        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 flatAcceleration = new Vector3(acceleration.x, 0f, acceleration.z);

        // Speed-based tilt multiplier for more dramatic effect at higher speeds
        float speedMultiplier = 1f + (Mathf.Clamp01(flatVelocity.magnitude / maxSpeedForTilt) * speedBasedTiltMultiplier);

        // Banking tilt (enhanced)
        float velocityTiltZ = 0f;
        if (flatVelocity.magnitude > velocityThreshold)
        {
            Vector3 right = Vector3.Cross(Vector3.up, flatVelocity.normalized);
            float lateralAccel = Vector3.Dot(flatAcceleration, right);
            velocityTiltZ = Mathf.Clamp(lateralAccel * velocityTiltAngle * speedMultiplier,
                                       -velocityTiltAngle * speedMultiplier,
                                       velocityTiltAngle * speedMultiplier);
        }

        // Acceleration tilt (enhanced)
        float accelerationTiltX = 0f;
        if (flatVelocity.magnitude > velocityThreshold)
        {
            float forwardAccel = Vector3.Dot(flatAcceleration, flatVelocity.normalized);
            accelerationTiltX = Mathf.Clamp(-forwardAccel * accelerationTiltAngle * speedMultiplier,
                                           -accelerationTiltAngle * speedMultiplier,
                                           accelerationTiltAngle * speedMultiplier);
        }

        // Calculate target tilt
        Vector3 targetTilt = new Vector3(accelerationTiltX, 0f, velocityTiltZ);

        // --- Falling Downward Camera Tilt ---
        float fallingTiltX = 0f;
        if (velocity.y < -1f) // Adjust threshold as needed
        {
            // The more negative the velocity, the more downward the camera looks (up to a max)
            fallingTiltX = Mathf.Clamp(Mathf.Abs(velocity.y) * 6f, 0f, 90f); // degrees max tilt
            targetTilt.x += fallingTiltX;
        }

        // Apply impact reduction
        float tiltMultiplier = 1f - (impactInfluence * impactTiltReduction);
        targetTilt *= tiltMultiplier;

        // Add damping for smoother transitions
        targetTilt = Vector3.Lerp(lastTargetTilt, targetTilt, tiltDamping);
        lastTargetTilt = targetTilt;

        // Clamp X tilt (up/down)
        targetTilt.x = Mathf.Clamp(targetTilt.x, -maxXTilt, maxXTilt);

        // Use dynamic smooth time - longer during impacts and generally smoother
        float dynamicSmoothTime = tiltSmoothTime + (impactInfluence * 0.4f);
        currentTilt = Vector3.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, dynamicSmoothTime);

        // Clamp X tilt (up/down)
        currentTilt.x = Mathf.Clamp(currentTilt.x, -maxXTilt, maxXTilt);

        transform.localRotation = Quaternion.Euler(currentTilt);
    }
}
