using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossMarble : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 20f;
    public float maxSpeed = 500f;
    public float marbleMass = 5f; // Increased mass for more weight

    private Rigidbody rb;
    private float inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = marbleMass; // Set the mass here
    }

    void Update()
    {
        // Use standard axis (A = -1, D = 1)
        inputX = -Input.GetAxis("Horizontal");

    }

    void FixedUpdate()
    {
        // Apply force based on input
        if (Mathf.Abs(inputX) > 0.01f)
        {
            rb.AddForce(Vector3.right * inputX * moveForce, ForceMode.Acceleration);
        }

        // Clamp only horizontal speed, allow vertical velocity
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedHorizontal = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(
                clampedHorizontal.x,
                rb.linearVelocity.y, // preserve vertical velocity
                clampedHorizontal.z
            );
        }

        // Optionally, apply extra gravity for more weight
        rb.AddForce(Physics.gravity * (marbleMass - 1), ForceMode.Acceleration);
    }
}
