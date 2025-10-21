using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Laser Settings")]
    public float maxLaserDistance = 50f;
    public float laserExtendSpeed = 50f; // This value will be multiplied by 10 in the code.
    public float laserOnDuration = 0.5f;
    public int laserTimingType = 1;

    [Header("References")]
    public LineRenderer lineRenderer;

    private float timer;
    private bool isLaserOn = false;
    private float currentLaserLength = 0f;
    private float activeTimer = 0f;
    private LayerMask collisionLayers;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Define collision layers in code for reliability.
        collisionLayers = LayerMask.GetMask("Default", "Wall", "Player");

        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        int currentSecond = Mathf.FloorToInt(timer);

        if (!isLaserOn && (currentSecond % 5 + 1) == laserTimingType)
        {
            ActivateLaser();
        }

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
    }

    void DeactivateLaser()
    {
        isLaserOn = false;
        lineRenderer.enabled = false;
    }

    void UpdateLaserBeam()
    {
        // 1. Determine the visual length, multiplying the Inspector speed by 10.
        // --- SPEED MULTIPLIER ADDED HERE ---
        float effectiveSpeed = laserExtendSpeed * 10f;
        currentLaserLength = Mathf.Min(maxLaserDistance, currentLaserLength + effectiveSpeed * Time.deltaTime);

        Vector3 startPoint = transform.position;
        Vector3 direction = transform.forward;
        Vector3 endPoint;

        // 2. Perform a raycast that matches the visual's current length.
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, currentLaserLength, collisionLayers))
        {
            // The check found a collider. The laser's endpoint is the hit point.
            endPoint = hit.point;

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Laser hit Player! End level.");
                if (EndLevelUIManager.Instance != null)
                {
                    EndLevelUIManager.Instance.ShowEndScreen(float.MaxValue);
                }
                DeactivateLaser();
            }
        }
        else
        {
            // The check found nothing. The laser's endpoint is its full visual length.
            endPoint = startPoint + direction * currentLaserLength;
        }

        // 3. Update the LineRenderer to show the result.
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // The debug visualizer has been removed.
    }
}