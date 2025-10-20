using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Laser Settings")]
    public float maxLaserDistance = 50f;         // Max range of the laser
    public float laserExtendSpeed = 50f;         // How fast the laser visually extends
    public float laserOnDuration = 0.5f;         // How long the laser stays visible when on
    public int laserTimingType = 1;              // 1 = trigger every 1 second, 2 = every 2 seconds

    [Header("References")]
    public LineRenderer lineRenderer;            // Assign your laser line here
    public Transform laserStartTransform;        // Assign your parent or start object here

    private float timer;
    private bool isLaserOn = false;
    private float currentLaserLength = 0f;
    private float activeTimer = 0f;
    private bool hitPlayerFlag = false;          // True if player was hit this cycle

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = false; // laser off at start
        lineRenderer.useWorldSpace = false;
    }

    void Update()
    {
        // Global repeating time pattern (based on seconds)
        timer += Time.deltaTime;
        int second = Mathf.FloorToInt(timer) % 2 + 1; // alternates between 1 and 2 seconds pattern

        // Activate laser depending on timing type
        if (!isLaserOn && second == laserTimingType)
        {
            ActivateLaser();
        }

        // Handle laser while it's active
        if (isLaserOn)
        {
            activeTimer += Time.deltaTime;
            UpdateLaserBeam();

            if (activeTimer >= laserOnDuration)
            {
                DeactivateLaser();
            }
        }
    }

    void ActivateLaser()
    {
        isLaserOn = true;
        lineRenderer.enabled = true;
        currentLaserLength = 0f;
        activeTimer = 0f;
        hitPlayerFlag = false;
    }

    void DeactivateLaser()
    {
        isLaserOn = false;
        lineRenderer.enabled = false;
    }

    void UpdateLaserBeam()
    {
        currentLaserLength += laserExtendSpeed * Time.deltaTime;
        if (currentLaserLength > maxLaserDistance)
            currentLaserLength = maxLaserDistance;

        // Always start at local origin
        Vector3 localStartPoint = Vector3.zero;

        // Use local forward direction
        Vector3 localDirection = Vector3.forward;

        // Raycast in world space from this object's position and forward
        Vector3 worldStartPoint = transform.position;
        Vector3 worldDirection = transform.forward;

        if (Physics.Raycast(worldStartPoint, worldDirection, out RaycastHit hit, currentLaserLength))
        {
            Vector3 localEndPoint = transform.InverseTransformPoint(hit.point);
            lineRenderer.SetPosition(0, localStartPoint);
            lineRenderer.SetPosition(1, localEndPoint);

            if (!hitPlayerFlag && hit.collider.CompareTag("Player"))
            {
                hitPlayerFlag = true;
                Debug.Log("Laser hit Player! End level.");
                // Example: LevelManager.Instance.EndLevel();
            }
        }
        else
        {
            // No hit — draw full extended laser in local space
            Vector3 localEndPoint = localStartPoint + localDirection * currentLaserLength;
            lineRenderer.SetPosition(0, localStartPoint);
            lineRenderer.SetPosition(1, localEndPoint);
        }
    }
}
