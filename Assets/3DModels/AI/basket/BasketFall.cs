using UnityEngine;

public class BasketFall : MonoBehaviour
{
    [SerializeField] private float timerDuration = 10f;
    [SerializeField] private float moveSpeed = 1f; // Speed of movement after timer ends
    [SerializeField] private float moveDistance = 2f; // Distance to move
    [SerializeField] private Vector3 moveDirection = Vector3.forward; // Direction to move (normalized automatically)

    private float timer;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool shouldMove = false;
    private float moveProgress = 0f;

    void Start()
    {
        timer = timerDuration;
        initialPosition = transform.position;

        // Calculate target position based on direction and distance
        targetPosition = initialPosition + moveDirection.normalized * moveDistance;
    }

    void Update()
    {
        // Countdown timer
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                Debug.Log("10 seconds have passed! Starting to move...");
                shouldMove = true;
            }
        }

        // Only start moving after timer reaches zero
        if (shouldMove && moveProgress < 1f)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            moveProgress = Mathf.Clamp01(moveProgress);

            // Interpolate position
            transform.position = Vector3.Lerp(initialPosition, targetPosition, moveProgress);
        }
    }
}
