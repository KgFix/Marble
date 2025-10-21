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
    [Header("Collision Layers")]
    [Tooltip("Layers that can stop the laser (e.g., Default, Wall, Player if player should block the beam)")]
    public LayerMask occluderLayers;
    [Tooltip("Layers considered as player for hit detection")]
    public LayerMask playerLayers;
    [Tooltip("Radius of the overlap capsule used to detect the player along the beam")]
    [SerializeField] private float playerHitRadius = 0.12f;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Defaults if not set in Inspector
        if (occluderLayers.value == 0)
            occluderLayers = LayerMask.GetMask("Default", "Wall", "Player");
        if (playerLayers.value == 0)
            playerLayers = LayerMask.GetMask("Player", "Default");

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
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, currentLaserLength, occluderLayers))
        {
            // The check found a collider. The laser's endpoint is the hit point.
            endPoint = hit.point;
        }
        else
        {
            // The check found nothing. The laser's endpoint is its full visual length.
            endPoint = startPoint + direction * currentLaserLength;
        }

        // 2b. Now check along the visible segment for the player using a small capsule (robust to thin misses)
    float radius = Mathf.Max(0.01f, playerHitRadius);
        // Build capsule points slightly inside the beam to avoid missing endpoints
        Vector3 a = startPoint + direction * 0.02f;
        Vector3 b = endPoint - direction * 0.02f;
        if (Vector3.Distance(a, b) < 0.01f)
        {
            b = a + direction * 0.02f;
        }
        var hits = Physics.OverlapCapsule(a, b, radius, playerLayers);
        if (hits != null && hits.Length > 0)
        {
            bool playerHit = false;
            foreach (var h in hits)
            {
                if (h != null && (h.CompareTag("Player") || ((1 << h.gameObject.layer) & playerLayers) != 0))
                {
                    playerHit = true;
                    break;
                }
            }
            if (playerHit)
            {
                if (!GameState.IsCompleted)
                {
                    Debug.Log("Laser hit Player! End level.");
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
                        Debug.LogWarning("Laser: No End Screen UI found (Level2/GameEndScreen/EndScreenUI). Ensure an end screen exists in the scene.");
                }
                DeactivateLaser();
            }
        }

        // 3. Update the LineRenderer to show the result.
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // The debug visualizer has been removed.
    }
}