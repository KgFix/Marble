using UnityEngine;

public class MarbleCameraY : MonoBehaviour
{
    public float tiltAngle = 20f;
    public float tiltSpeed = 5f;

    void LateUpdate()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Only read player input if the level is not completed
        if (!GameState.IsCompleted)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        // Opposite tilt direction
        float tiltX = -verticalInput * tiltAngle;
        float tiltZ = horizontalInput * tiltAngle;

        Quaternion targetTilt = Quaternion.Euler(tiltX, 0f, tiltZ);

        // Smoothly tilt toward target
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetTilt,
            tiltSpeed * Time.deltaTime
        );
    }
}
