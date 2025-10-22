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

    private GameManager3 gameManager3;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = marbleMass; // Set the mass here

        // Cache the GameManager3 instance
        gameManager3 = FindObjectOfType<GameManager3>();
        if (gameManager3 == null)
            Debug.LogWarning("BossMarble: GameManager3 not found in scene.");
    }

    void Update()
    {
        inputX = -Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(inputX) > 0.01f)
        {
            rb.AddForce(Vector3.right * inputX * moveForce, ForceMode.Acceleration);
        }

        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedHorizontal = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(
                clampedHorizontal.x,
                rb.linearVelocity.y,
                clampedHorizontal.z
            );
        }

        rb.AddForce(Physics.gravity * (marbleMass - 1), ForceMode.Acceleration);
    }

    private bool IsGrounded()
    {
        float rayLength = 0.6f;
        return Physics.Raycast(transform.position, Vector3.down, rayLength + 0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("BossMarble collided with: " + collision.gameObject.name);

        // Only check for collisions with Right.Hand.Collider or Left.Hand.Collider
        if (
            collision.gameObject.name == "Right.Hand.Collider" ||
            collision.gameObject.name == "Left.Hand.Collider"
        )
        {
            if (!GameState.IsCompleted)
            {
                Debug.Log("BossMarble hit by hand! End level.");
                GameState.SetVictory(false);
                GameState.CompleteLevel();

                float failTime = GameState.LevelTime;
                var endScreen = FindObjectOfType<EndScreenUI3>(includeInactive: true);
                if (endScreen != null)
                    endScreen.ShowEndScreen(false, failTime);
                else if (!EndScreenHelper.TryShowEndScreen())
                    Debug.LogWarning("BossMarble: No End Screen UI found. Ensure an end screen exists in the scene.");
            }
        }
    }
}
