using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformTilt : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float tiltAngle = 30f;   // Max tilt angle
    public float tiltSpeed = 5f;    // Smoothness

    private Quaternion targetRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // platform shouldn't be moved by physics
    }

    void FixedUpdate()
    {
        float targetX = 0f;

        if (Input.GetKey(KeyCode.D))
            targetX = tiltAngle;
        //else if (Input.GetKey(KeyCode.A))
        //    targetX = -tiltAngle;

        targetRotation = Quaternion.Euler(targetX, 0f, 0f);

        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * tiltSpeed));
    }
}
