using UnityEngine;

public class PlatformTilt : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float tiltAngle = 30f;   // Max tilt angle on X
    public float tiltSpeed = 5f;    // Smoothness

    private Quaternion targetRotation;

    void Update()
    {
        float targetX = 0f;

        if (Input.GetKey(KeyCode.D))
            targetX = tiltAngle;
        else if (Input.GetKey(KeyCode.A))
            targetX = -tiltAngle;

        // Target rotation
        targetRotation = Quaternion.Euler(targetX, 0f, 0f);

        // Smooth rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}
