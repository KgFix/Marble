using UnityEngine;

public class MarbleCameraY : MonoBehaviour
{
    public float tiltAngle = 20f;
    public float tiltSpeed = 5f;

    void LateUpdate()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (GameState.InputEnabled)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        float tiltX = -verticalInput * tiltAngle;
        float tiltZ = horizontalInput * tiltAngle;

        Quaternion targetTilt = Quaternion.Euler(tiltX, 0f, tiltZ);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetTilt,
            tiltSpeed * Time.deltaTime
        );
    }

}
