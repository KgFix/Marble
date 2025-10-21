using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarbleController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 20f;
    public float maxSpeed = 500f;
    public float jumpForce = 10f;
    public float marbleMass = 5f; // Increased mass for more weight

    [Header("Camera Settings")]
    public Camera mainCamera;
    public float tiltAngle = 20f;
    public float tiltSpeed = 5f;

    private Rigidbody rb;
    private float inputX;
    private bool jumpRequested = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = marbleMass; // Set the mass here
    }

    void Update()
    {
        inputX = -Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // Apply force based on input
        if (Mathf.Abs(inputX) > 0.01f)
        {
            if (mainCamera != null)
            {
                Vector3 camRight = mainCamera.transform.right;
                camRight.y = 0;
                camRight.Normalize();
                rb.AddForce(camRight * -inputX * moveForce, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(Vector3.right * -inputX * moveForce, ForceMode.Acceleration);
            }
        }

        // Clamp only horizontal speed, allow vertical velocity for jumps
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


        // Camera tilt handling
        if (mainCamera != null)
        {
            float targetTilt = -inputX * tiltAngle;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetTilt);
            mainCamera.transform.localRotation = Quaternion.Slerp(
                mainCamera.transform.localRotation,
                targetRotation,
                Time.fixedDeltaTime * tiltSpeed
            );
        }

        // Optionally, apply extra gravity for more weight
         rb.AddForce(Physics.gravity * (marbleMass - 1), ForceMode.Acceleration);
    }
}
