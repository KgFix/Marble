using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossMarble : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 20f;
    public float maxSpeed = 500f;
    public float marbleMass = 5f; // Increased mass for more weight

    [Header("Jump Settings")]
    public float jumpForce = 10f; // Adjustable jump height in inspector

    private Rigidbody rb;
    private float inputX;
    private bool jumpRequested = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = marbleMass; // Set the mass here
    }

    void Update()
    {
        // Use standard axis (A = -1, D = 1)
        inputX = -Input.GetAxis("Horizontal");

        // Check for jump input
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // Apply force based on input
        if (Mathf.Abs(inputX) > 0.01f)
        {
            rb.AddForce(Vector3.right * inputX * moveForce, ForceMode.Acceleration);
        }

        // Jump logic
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }

        // Clamp only horizontal speed, allow vertical velocity
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedHorizontal = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(
                clampedHorizontal.x,
                rb.linearVelocity.y, // preserve vertical velocity
                clampedHorizontal.z
            );
        }

        // Optionally, apply extra gravity for more weight
        rb.AddForce(Physics.gravity * (marbleMass - 1), ForceMode.Acceleration);
    }

    // Simple ground check using a raycast
    private bool IsGrounded()
    {
        // Adjust the ray length and offset as needed for your marble size
        float rayLength = 0.6f;
        return Physics.Raycast(transform.position, Vector3.down, rayLength + 0.1f);
    }

    // Detect collision with the player
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("BossMarble collided with: " + collision.gameObject.name);

        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!GameState.IsCompleted)
            {
                Debug.Log("BossMarble hit Player! End level.");
                GameState.SetVictory(false);
                GameState.CompleteLevel();

                // Prefer the Level 2 end screen if present, else fallback to manager
                var end2 = SceneUtil.FindInScene<EndGameScreenforlevel2>(includeInactive: true);
                float failTime = GameState.LevelTime; // show actual elapsed time on failure
                if (end2 != null)
                    end2.ShowEndScreen(failTime, false);
                else if (EndLevelUIManager.Instance != null)
                    EndLevelUIManager.Instance.ShowEndScreen(failTime);
                else if (!EndScreenHelper.TryShowEndScreen())
                    Debug.LogWarning("BossMarble: No End Screen UI found (Level2/GameEndScreen/EndScreenUI). Ensure an end screen exists in the scene.");
            }
        }
    }
}
