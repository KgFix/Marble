using UnityEngine;

public class TankMover : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotationSpeed = 90f; // Degrees per second
    public float stepDuration = 2f;

    public float forwardDuration = 2f;
    public float rightDuration = 2f;
    public float leftDuration = 2f;
    public float backwardDuration = 2f;

    private float stepTimer;
    private int stepIndex;

    // Directions in world space
    private Vector3[] directions;
    private Quaternion targetRotation;
    private bool isRotating = true;

    void Start()
    {
        directions = new Vector3[]
        {
            Vector3.forward,
            Vector3.right,
            Vector3.left,
            Vector3.back
        };

        stepIndex = 0;
        SetTargetRotation();
        stepTimer = GetCurrentStepDuration();
        isRotating = true;
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate towards target direction
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (angle < 1f)
            {
                transform.rotation = targetRotation; // Snap to exact
                isRotating = false; // Start moving
            }
        }
        else
        {
            // Move in the current direction
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // Countdown timer only while moving
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                // Next step: set new direction and start rotating
                stepIndex = (stepIndex + 1) % directions.Length;
                SetTargetRotation();
                stepTimer = GetCurrentStepDuration();
                isRotating = true;
            }
        }
    }

    void SetTargetRotation()
    {
        targetRotation = Quaternion.LookRotation(directions[stepIndex]);
    }

    float GetCurrentStepDuration()
    {
        switch (stepIndex)
        {
            case 0: return forwardDuration;
            case 1: return rightDuration;
            case 2: return leftDuration;
            case 3: return backwardDuration;
            default: return stepDuration;
        }
    }
}
