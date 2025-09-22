using UnityEngine;

public class Marble : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphere;
    public GameObject cameraX;
    public GameObject cameraY;

    [Header("Normal Movement Settings")]
    [SerializeField] private float normalMaxMoveSpeed = 0.5f;
    [SerializeField] private float normalAcceleration = 1.5f;
    [SerializeField] private float normalDeceleration = 2.5f;

    [Header("Sprint (Shift) Movement Settings")]
    [SerializeField] private float sprintMaxMoveSpeed = 15f;
    [SerializeField] private float sprintAcceleration = 3f;
    [SerializeField] private float sprintDeceleration = 5f;

    [Header("Speed Boost Settings")]
    [SerializeField] private float speedBoostMultiplier = 40f; // For speed pads only
    [SerializeField] private float speedBoostDuration = 2f;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float currentSpeed = 0f;

    // Speed boost variables
    private bool isSpeedBoosted = false;
    private float speedBoostTimer = 0f;
    private float originalMaxSpeed;

    void Start()
    {
        originalMaxSpeed = normalMaxMoveSpeed;
    }

    void Update()
    {
        GetInput();
        UpdateSpeedBoost();
    }

    void FixedUpdate()
    {
        MoveSphere();
    }

    void GetInput()
    {
        if (GameState.InputEnabled)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        else
        {
            horizontalInput = 0f;
            verticalInput = 0f;
        }
    }

    private bool IsSprinting()
    {
        return GameState.InputEnabled && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    private float GetCurrentMaxSpeed()
    {
        if (isSpeedBoosted)
            return originalMaxSpeed * speedBoostMultiplier;
        return IsSprinting() ? sprintMaxMoveSpeed : normalMaxMoveSpeed;
    }

    private float GetCurrentAcceleration()
    {
        return IsSprinting() ? sprintAcceleration : normalAcceleration;
    }

    private float GetCurrentDeceleration()
    {
        return IsSprinting() ? sprintDeceleration : normalDeceleration;
    }

    void UpdateSpeedBoost()
    {
        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0f)
            {
                EndSpeedBoost();
            }
        }
    }

    public void ApplySpeedBoost()
    {
        isSpeedBoosted = true;
        speedBoostTimer = speedBoostDuration;
    }

    void EndSpeedBoost()
    {
        isSpeedBoosted = false;
    }

    void MoveSphere()
    {
        // Camera-relative movement
        Vector3 forward = cameraX.transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = cameraY.transform.right; right.y = 0f; right.Normalize();

        moveDirection = (forward * verticalInput + right * horizontalInput);

        float moveSpeed = GetCurrentMaxSpeed();
        float acceleration = GetCurrentAcceleration();
        float deceleration = GetCurrentDeceleration();

        // Acceleration / Deceleration
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // Apply force in a uniform direction
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 force = moveDirection.normalized * currentSpeed;
            sphere.AddForce(force, ForceMode.Force);
        }

        // Clamp velocity so diagonals aren't faster
        if (sphere.linearVelocity.magnitude > moveSpeed)
        {
            sphere.linearVelocity = sphere.linearVelocity.normalized * moveSpeed;
        }
    }
}
