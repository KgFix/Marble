using UnityEngine;

public class Marble : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphere;
    public GameObject cameraX;
    public GameObject cameraY;

    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 6f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float deceleration = 5f;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float currentSpeed = 0f;

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        MoveSphere();
    }

    void GetInput()
    {
        // Stop reading player input if level is completed
        if (!GameState.IsCompleted)
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

        // Clamp velocity so diagonals aren’t faster
        if (sphere.linearVelocity.magnitude > maxMoveSpeed)
        {
            sphere.linearVelocity = sphere.linearVelocity.normalized * maxMoveSpeed;
        }
    }
}
