using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarblePhysicsController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 20f;        // Maximum rolling speed
    public float accelLimit = 10f;      // Maximum acceleration force applied

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // smooth movement
    }

    void FixedUpdate()
    {
        // Clamp max velocity
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Optional: limit acceleration so marble doesn’t explode on steep tilts
        Vector3 acceleration = Physics.gravity;

        if (acceleration.magnitude > accelLimit)
        {
            acceleration = acceleration.normalized * accelLimit;
        }

        // Apply controlled acceleration force
        rb.AddForce(acceleration, ForceMode.Acceleration);
    }
}
