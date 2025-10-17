using UnityEngine;
using System.Collections;

public class DashPanel : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 35f; // Horizontal dash force
    public float upwardForce = 30f; // Triple jump height
    public float upwardForceDuration = 0.3f; // Duration to apply upward force
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Direction")]
    public bool useForwardDirection = true;
    public Vector3 customDirection = Vector3.forward;

    [Header("Visual Effects")]
    public GameObject dashModel;
    public ParticleSystem dashEffect;
    public Material normalMaterial;
    public Material activeMaterial;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dashSound;

    [Header("Cooldown")]
    public float cooldownTime = 0.5f;
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

        lastActivationTime = -cooldownTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastActivationTime < cooldownTime)
            return;

        if (other.CompareTag("Player") || other.name.Contains("Ball"))
        {
            Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();

            if (ballRigidbody != null)
            {
                ApplyDashForce(ballRigidbody);
                StartCoroutine(ApplyUpwardAcceleration(ballRigidbody));

                // Apply speed boost to the marble
                Marble marble = other.GetComponent<Marble>();
                if (marble != null)
                {
                    marble.ApplySpeedBoost();
                }

                PlayDashEffect();
                lastActivationTime = Time.time;
            }
        }
    }

    void ApplyDashForce(Rigidbody ballRigidbody)
    {
        Vector3 dashDirection = useForwardDirection ? transform.forward : customDirection.normalized;

        // Clear existing velocity for consistent dash
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);
    }

    IEnumerator ApplyUpwardAcceleration(Rigidbody ballRigidbody)
    {
        float elapsed = 0f;
        float interval = 0.02f; // Physics update interval
        int steps = Mathf.CeilToInt(upwardForceDuration / interval);
        float forcePerStep = upwardForce / steps;

        while (elapsed < upwardForceDuration)
        {
            ballRigidbody.AddForce(Vector3.up * forcePerStep, ForceMode.VelocityChange);
            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    void PlayDashEffect()
    {
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        if (dashEffect != null)
        {
            dashEffect.Play();
        }

        if (dashModel != null)
        {
            StartCoroutine(DashAnimation());
        }
    }

    IEnumerator DashAnimation()
    {
        if (panelRenderer != null && activeMaterial != null)
        {
            panelRenderer.material = activeMaterial;
        }

        float animationTime = 0.3f;
        float elapsedTime = 0f;
        Vector3 expandedScale = originalScale * 1.1f;

        while (elapsedTime < animationTime)
        {
            float progress = elapsedTime / animationTime;
            float scaleMultiplier = Mathf.Sin(progress * Mathf.PI);
            Vector3 currentScale = Vector3.Lerp(originalScale, expandedScale, scaleMultiplier);
            dashModel.transform.localScale = currentScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dashModel.transform.localScale = originalScale;
        if (panelRenderer != null && normalMaterial != null)
        {
            panelRenderer.material = normalMaterial;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 direction = useForwardDirection ? transform.forward : customDirection.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction * 3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + direction * 3f, 0.3f);
    }
}
