using UnityEngine;

public class Marble : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphere;
    public GameObject cameraX;
    public GameObject cameraY;

    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 0.5f;
    [SerializeField] private float acceleration = 0.05f;
    [SerializeField] private float deceleration = 0.1f;

    [Header("Speed Boost Settings")]
    [SerializeField] private float speedBoostMultiplier = 40f; // How much faster during boost
    [SerializeField] private float speedBoostDuration = 2f; // How long the boost lasts

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
        originalMaxSpeed = maxMoveSpeed;
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
        maxMoveSpeed = originalMaxSpeed * speedBoostMultiplier;
    }

    void EndSpeedBoost()
    {
        isSpeedBoosted = false;
        maxMoveSpeed = originalMaxSpeed;
    }

    void MoveSphere()
    {
        // Camera-relative movement
        Vector3 forward = cameraX.transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = cameraY.transform.right; right.y = 0f; right.Normalize();

        moveDirection = (forward * verticalInput + right * horizontalInput);

        // Acceleration / Deceleration
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxMoveSpeed, acceleration * Time.fixedDeltaTime);
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
        if (sphere.linearVelocity.magnitude > maxMoveSpeed)
        {
            sphere.linearVelocity = sphere.linearVelocity.normalized * maxMoveSpeed;
        }
    }
}
