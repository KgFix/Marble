using UnityEngine;
using System.Collections;

public class DashPanel : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float upwardForce = 5f; // Helps get up ramps
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Direction")]
    public bool useForwardDirection = true; // Use panel's forward direction
    public Vector3 customDirection = Vector3.forward; // Custom direction if needed

    [Header("Visual Effects")]
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

        // Add upward component for ramps
        dashDirection += Vector3.up * (upwardForce / dashForce);
        dashDirection = dashDirection.normalized;

        // POWERFUL VERSION: Clear existing velocity first, then boost
        ballRigidbody.linearVelocity = Vector3.zero; // Stop current movement
        ballRigidbody.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        // Alternative: Keep some existing momentum
        // Vector3 existingVelocity = ballRigidbody.velocity;
        // ballRigidbody.velocity = existingVelocity * 0.3f + dashDirection * dashForce;
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