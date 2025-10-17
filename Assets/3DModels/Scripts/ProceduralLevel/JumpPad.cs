using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class JumpPad : MonoBehaviour
{
    [Header("Spring Power")]
    [Tooltip("Controls the strength of the jump pad's upward force.")]
    [Range(1f, 400f)] // Increased max for more flexibility
    public float power = 60f; // Increased default for a stronger jump

    private void Reset()
    {
        var collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * power, ForceMode.Impulse); // Changed to Impulse for a faster, more realistic boost
        }
    }
}
