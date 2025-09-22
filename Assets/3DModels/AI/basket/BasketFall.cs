using UnityEngine;

public class BasketFall : MonoBehaviour
{
    [SerializeField] private float tiltAngle = 80f;
    [SerializeField] private float timerDuration = 10f;
    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 0f, -0.5f); // Adjust Z for front edge

    private float timer;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Vector3 initialPosition;

    void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        timer = timerDuration;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            float t = 1f - (timer / timerDuration);

            // Calculate rotation
            Quaternion currentRotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            // Calculate rotated pivot offset
            Vector3 rotatedOffset = currentRotation * pivotOffset;

            // Apply rotation and position offset
            transform.rotation = currentRotation;
            transform.position = initialPosition + (rotatedOffset - (initialRotation * pivotOffset));

            if (timer <= 0f)
            {
                Debug.Log("10 seconds have passed!");
            }
        }
    }
}
