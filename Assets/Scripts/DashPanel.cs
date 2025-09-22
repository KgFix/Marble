using UnityEngine;
using System.Collections;

public class DashPanel : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 50f; // Increased default
    public float upwardForce = 15f; // Increased default
    public ForceMode forceMode = ForceMode.VelocityChange; // Changed default
    public bool clearExistingVelocity = true; // New option

    [Header("Performance")]
    public bool useFixedTimestep = true; // Use FixedUpdate timing for consistent physics

    [Header("Ramp Specific Settings")]
    public float rampAngle = 0f; // The angle of the ramp this panel is on
    public bool autoCalculateForce = false; // Automatically adjust force based on ramp angle
    public bool useForwardDirection = true; // Use panel's forward direction
    public Vector3 customDirection = Vector3.forward; // Custom direction if needed
    public GameObject dashModel; // The visual part that will animate
    public ParticleSystem dashEffect; // Speed lines or boost effect
    public Material normalMaterial;
    public Material activeMaterial; // Glowing material when activated

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dashSound;

    [Header("Cooldown")]
    public float cooldownTime = 0.5f; // Prevent spam boosting
    private float lastActivationTime;

    private Renderer panelRenderer;
    private Vector3 originalScale;

    void Start()
    {
        if (dashModel != null)
        {
            originalScale = dashModel.transform.localScale;
            panelRenderer = dashModel.GetComponent<Renderer>();
        }

        lastActivationTime = -cooldownTime; // Allow immediate first use
    }

    void OnTriggerEnter(Collider other)
    {
        // Check cooldown
        if (Time.time - lastActivationTime < cooldownTime)
            return;

        if (other.CompareTag("Player") || other.name.Contains("Ball"))
        {
            Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();

            if (ballRigidbody != null)
            {
                ApplyDashForce(ballRigidbody);
                PlayDashEffect();
                lastActivationTime = Time.time;
            }
        }
    }

    void ApplyDashForce(Rigidbody ballRigidbody)
    {
        // Calculate dash direction
        Vector3 dashDirection;

        if (useForwardDirection)
        {
            // Use the panel's forward direction (blue arrow in Scene view)
            dashDirection = transform.forward;
        }
        else
        {
            // Use custom direction
            dashDirection = customDirection.normalized;
        }

        // Auto-calculate forces based on ramp angle if enabled
        float finalDashForce = dashForce;
        float finalUpwardForce = upwardForce;

        if (autoCalculateForce && rampAngle > 0)
        {
            // Increase force based on ramp steepness
            float angleMultiplier = 1f + (rampAngle / 45f); // 45� = 2x force
            finalDashForce *= angleMultiplier;
            finalUpwardForce *= angleMultiplier;
        }

        // Add upward component for ramps
        dashDirection += Vector3.up * (finalUpwardForce / finalDashForce);
        dashDirection = dashDirection.normalized;

        // Clear existing velocity if option is enabled
        if (clearExistingVelocity)
        {
            ballRigidbody.linearVelocity = Vector3.zero;
        }

        // Apply powerful boost
        ballRigidbody.AddForce(dashDirection * finalDashForce, forceMode);

        // Extra boost for steep ramps - apply additional upward force
        if (finalUpwardForce > 0)
        {
            ballRigidbody.AddForce(Vector3.up * finalUpwardForce, ForceMode.VelocityChange);
        }

        Debug.Log($"Dash applied: Force={finalDashForce}, Upward={finalUpwardForce}, Angle={rampAngle}�");
    }

    void PlayDashEffect()
    {
        // Play sound
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        // Play particle effect
        if (dashEffect != null)
        {
            dashEffect.Play();
        }

        // Visual animation
        if (dashModel != null)
        {
            StartCoroutine(DashAnimation());
        }
    }

    IEnumerator DashAnimation()
    {
        // Flash bright material
        if (panelRenderer != null && activeMaterial != null)
        {
            panelRenderer.material = activeMaterial;
        }

        // Scale animation
        float animationTime = 0.3f;
        float elapsedTime = 0f;
        Vector3 expandedScale = originalScale * 1.1f;

        while (elapsedTime < animationTime)
        {
            float progress = elapsedTime / animationTime;

            // Scale up then down
            float scaleMultiplier = Mathf.Sin(progress * Mathf.PI);
            Vector3 currentScale = Vector3.Lerp(originalScale, expandedScale, scaleMultiplier);
            dashModel.transform.localScale = currentScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset
        dashModel.transform.localScale = originalScale;
        if (panelRenderer != null && normalMaterial != null)
        {
            panelRenderer.material = normalMaterial;
        }
    }

    // Helper method to visualize direction in Scene view
    void OnDrawGizmosSelected()
    {
        Vector3 direction = useForwardDirection ? transform.forward : customDirection.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction * 3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + direction * 3f, 0.3f);
    }
}