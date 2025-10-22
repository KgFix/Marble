using UnityEngine;

public class Level1Marble : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphere;
    // Removed: public GameObject cameraX; // Not needed with Level1Camera
    // Removed: public GameObject cameraY; // Not needed with Level1Camera
    public Level1Camera mainCamera; // New reference for combined camera script

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
        smoothedMaxSpeedCap = Level1GetCurrentMaxSpeed();

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
        Level1GetInput();
        Level1UpdateSpeedBoost();
        Level1HandleMovingAudio();
        Level1HandleSprintAudio();
    }

    void FixedUpdate()
    {
        Level1MoveSphere();
    }

    void Level1GetInput()
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

    private bool Level1IsSprinting()
    {
        return GameState.InputEnabled && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    private float Level1GetCurrentMaxSpeed()
    {
        if (isSpeedBoosted)
            return originalMaxSpeed * speedBoostMultiplier;
        return Level1IsSprinting() ? sprintMaxMoveSpeed : normalMaxMoveSpeed;
    }

    private float Level1GetCurrentAcceleration()
    {
        return Level1IsSprinting() ? sprintAcceleration : normalAcceleration;
    }

    private float Level1GetCurrentDeceleration()
    {
        return Level1IsSprinting() ? sprintDeceleration : normalDeceleration;
    }

    void Level1UpdateSpeedBoost()
    {
        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0f)
            {
                Level1EndSpeedBoost();
            }
        }
    }

    public void Level1ApplySpeedBoost()
    {
        isSpeedBoosted = true;
        speedBoostTimer = speedBoostDuration;
    }

    void Level1EndSpeedBoost()
    {
        isSpeedBoosted = false;
    }

    void Level1MoveSphere()
    {
        if (mainCamera == null) return;

        // Camera-relative movement
        // Now using the single Level1Camera component's properties
        Vector3 forward = mainCamera.CameraForwardFlat;
        Vector3 right = mainCamera.CameraRightFlat;

        moveDirection = (forward * verticalInput + right * horizontalInput);

        // Smooth the max speed cap so Shift press/release isn't instant
        float targetMaxSpeed = Level1GetCurrentMaxSpeed();
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
        float acceleration = Level1GetCurrentAcceleration();
        float deceleration = Level1GetCurrentDeceleration();

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

    void Level1HandleMovingAudio()
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
            smoothedMaxSpeedCap = Mathf.Max(smoothedMaxSpeedCap, Level1GetCurrentMaxSpeed());
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

    void Level1HandleSprintAudio()
    {
        bool sprinting = Level1IsSprinting() && (horizontalInput != 0f || verticalInput != 0f);

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