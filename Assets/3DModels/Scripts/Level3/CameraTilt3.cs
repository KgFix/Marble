using UnityEngine;

public class CameraTilt3 : MonoBehaviour
{
    private Vector3 initialEulerAngles; // Store the initial rotation
    public float tiltAmount = 20f;      // Maximum tilt angle
    public float tiltSmooth = 0.15f;    // Smoothing time (smaller = faster)

    private float currentTilt = 0f;
    private float targetTilt = 0f;
    private float tiltVelocity = 0f;    // Used by SmoothDamp

    void Start()
    {
        // Record the initial rotation of the camera
        initialEulerAngles = transform.eulerAngles;
        currentTilt = 0f;
    }

    void LateUpdate()
    {
        // Get horizontal input (A = -1, D = 1)
        float input = Input.GetAxis("Horizontal");
        targetTilt = input * tiltAmount;

        // Smoothly interpolate current tilt towards target tilt using SmoothDamp
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmooth);

        // Only apply tilt on the Z axis (roll), keep other rotations unchanged
        transform.eulerAngles = new Vector3(
            initialEulerAngles.x,
            initialEulerAngles.y,
            initialEulerAngles.z + currentTilt
        );
    }
}
