using UnityEngine;

public class Bumper : MonoBehaviour
{
    [Header("Bumper Settings")]
    public float bumperForce = 10f;
    public float upwardForce = 2f; // Optional: adds slight upward momentum

    [Header("Visual Feedback")]
    public GameObject bumperModel; // The visual part that will animate
    public float squashAmount = 0.8f;
    public float squashDuration = 0.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bumperSound;

    private Vector3 originalScale;

    void Start()
    {
        if (bumperModel != null)
            originalScale = bumperModel.transform.localScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the ball
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Ball"))
        {
            Rigidbody ballRigidbody = collision.rigidbody;

            if (ballRigidbody != null)
            {
                // Calculate the direction to push the ball (opposite of collision direction)
                Vector3 pushDirection = collision.transform.position - transform.position;
                pushDirection.y = 0; // Keep it horizontal, or remove this line if you want full 3D bumping
                pushDirection = pushDirection.normalized;

                // Add some upward force for more dynamic bouncing
                pushDirection += Vector3.up * (upwardForce / bumperForce);

                // Apply the force
                ballRigidbody.AddForce(pushDirection * bumperForce, ForceMode.Impulse);

                // Visual and audio feedback
                PlayBumperEffect();
            }
        }
    }

    void PlayBumperEffect()
    {
        // Play sound
        if (audioSource != null && bumperSound != null)
        {
            audioSource.PlayOneShot(bumperSound);
        }

        // Squash animation
        if (bumperModel != null)
        {
            StartCoroutine(SquashAnimation());
        }
    }

    System.Collections.IEnumerator SquashAnimation()
    {
        float elapsedTime = 0f;
        Vector3 squashedScale = new Vector3(originalScale.x, originalScale.y * squashAmount, originalScale.z);

        // Squash
        while (elapsedTime < squashDuration / 2)
        {
            bumperModel.transform.localScale = Vector3.Lerp(originalScale, squashedScale, elapsedTime / (squashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // Return to original
        while (elapsedTime < squashDuration / 2)
        {
            bumperModel.transform.localScale = Vector3.Lerp(squashedScale, originalScale, elapsedTime / (squashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bumperModel.transform.localScale = originalScale;
    }
}
