using UnityEngine;

public class Marble : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphere;
    public GameObject cameraX;
    public GameObject cameraY;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip sprintClip;

    [Header("Normal Movement Settings")]
    [SerializeField] private float normalMaxMoveSpeed = 0.5f;
    [SerializeField] private float normalAcceleration = 1.5f;
    [SerializeField] private float normalDeceleration = 2.5f;

    [Header("Sprint (Shift) Movement Settings")]
    [SerializeField] private float sprintMaxMoveSpeed = 15f;
    [SerializeField] private float sprintAcceleration = 3f;
    [SerializeField] private float sprintDeceleration = 5f;

    [Header("Speed Transition Settings")]
    [Tooltip("How fast the speed cap raises (units/sec) when entering sprint or speed boost.")]
    [SerializeField] private float speedCapIncreaseRate = 12f;
    [Tooltip("How fast the speed cap lowers (units/sec) when exiting sprint or speed boost.")]
    [SerializeField] private float speedCapDecreaseRate = 6f;

    [Header("Speed Boost Settings")]
    [SerializeField] private float speedBoostMultiplier = 40f; // For speed pads only
    [SerializeField] private float speedBoostDuration = 2f;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float currentSpeed = 0f;
    private float smoothedMaxSpeedCap = 0f;

    // Speed boost variables
    private bool isSpeedBoosted = false;
    private float speedBoostTimer = 0f;
    private float originalMaxSpeed;

    // AudioSources (created at runtime)
    private AudioSource moveSource;
    private AudioSource sprintSource;

    // Sprint audio state
    private bool wasSprinting = false;

    // Threshold to consider the marble as "moving"
    private const float movementThreshold = 0.1f;

    void Start()
    {
        originalMaxSpeed = normalMaxMoveSpeed;
        smoothedMaxSpeedCap = GetCurrentMaxSpeed();

        // Create and configure move audio source
        moveSource = gameObject.AddComponent<AudioSource>();
        moveSource.clip = moveClip;
        moveSource.loop = true;
        moveSource.playOnAwake = false;
        moveSource.volume = 0f;

        // Create and configure sprint audio source
        sprintSource = gameObject.AddComponent<AudioSource>();
        sprintSource.clip = sprintClip;
        sprintSource.loop = true;
        sprintSource.playOnAwake = false;
        sprintSource.volume = 1f;
    }

    void Update()
    {
        GetInput();
        UpdateSpeedBoost();
        HandleMovingAudio();
        HandleSprintAudio();
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

        // Smooth the max speed cap so Shift press/release isn't instant
        float targetMaxSpeed = GetCurrentMaxSpeed();
        if (isSpeedBoosted)
        {
            // Do not smooth during speed boost; keep full cap instantly to preserve jump distance
            smoothedMaxSpeedCap = targetMaxSpeed;
        }
        else if (targetMaxSpeed > smoothedMaxSpeedCap)
        {
            smoothedMaxSpeedCap = Mathf.MoveTowards(smoothedMaxSpeedCap, targetMaxSpeed, speedCapIncreaseRate * Time.fixedDeltaTime);
        }
        else
        {
            smoothedMaxSpeedCap = Mathf.MoveTowards(smoothedMaxSpeedCap, targetMaxSpeed, speedCapDecreaseRate * Time.fixedDeltaTime);
        }

        float moveSpeed = smoothedMaxSpeedCap;
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

        // Clamp velocity using smoothed cap so diagonals aren't faster and transitions aren't abrupt
        if (sphere.linearVelocity.magnitude > smoothedMaxSpeedCap)
        {
            sphere.linearVelocity = sphere.linearVelocity.normalized * smoothedMaxSpeedCap;
        }
    }

    void HandleMovingAudio()
    {
        if (moveSource == null || moveClip == null || sphere == null)
            return;

        float velocity = sphere.linearVelocity.magnitude;
        bool isMoving = velocity > movementThreshold;

            // Volume scales with velocity, clamp to [0,1]
            float maxExpectedSpeed = Mathf.Max(smoothedMaxSpeedCap, normalMaxMoveSpeed) * 1.2f; // Adjust as needed
        float targetVolume = Mathf.Clamp01(velocity / maxExpectedSpeed);

        if (isMoving)
        {
        // Nudge the cap upward so it doesn't feel delayed on boost start
        smoothedMaxSpeedCap = Mathf.Max(smoothedMaxSpeedCap, GetCurrentMaxSpeed());
            if (!moveSource.isPlaying)
                moveSource.Play();
            moveSource.volume = targetVolume;
        }
        else
        {
            if (moveSource.isPlaying)
                moveSource.Stop();
            moveSource.volume = 0f;
        }
    }

    void HandleSprintAudio()
    {
        bool sprinting = IsSprinting() && (horizontalInput != 0f || verticalInput != 0f);

        if (sprintSource == null || sprintClip == null)
            return;

        if (sprinting && !wasSprinting)
        {
            sprintSource.Play();
        }
        else if (!sprinting && wasSprinting)
        {
            sprintSource.Stop();
        }

        wasSprinting = sprinting;
    }
}
