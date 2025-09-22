using UnityEngine;

public class BasketFall : MonoBehaviour
{
    // Angle to tilt the basket (in degrees)
    [SerializeField] private float tiltAngle = 80f;

    // Timer duration in seconds
    [SerializeField] private float timerDuration = 10f;
    private float timer;

    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Store the initial upright rotation
        initialRotation = transform.rotation;

        // Calculate the target tilted rotation
        targetRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        // Initialize timer
        timer = timerDuration;
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            // Calculate interpolation factor (0 to 1)
            float t = 1f - (timer / timerDuration);

            // Interpolate rotation for a graceful fall
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            if (timer <= 0f)
            {
                // Timer finished, you can trigger an action here
                Debug.Log("10 seconds have passed!");
            }
        }
    }
}
