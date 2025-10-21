using UnityEngine;

public class Laser3 : MonoBehaviour
{
    [Header("Laser Settings")]
    public float maxLaserDistance = 50f;
    public float laserExtendSpeed = 50f; // This value will be multiplied by 10 in the code.
    [Tooltip("How long the laser stays on (seconds)")]
    public float laserOnDuration = 2f; // Default, but should be set by BossAnimations
    [Header("References")]
    public LineRenderer lineRenderer;

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

    [Header("Prediction Settings")]
    [Tooltip("How far ahead (in seconds) to predict the player's position")]
    [SerializeField] private float predictionTime = 0.5f;

    // For delayed tracking
    private Vector3? delayedTarget = null;

    public void SetTargetPosition(Vector3? target)
    {
        delayedTarget = target;
    }

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (occluderLayers.value == 0)
            occluderLayers = LayerMask.GetMask("Default", "Wall", "Player");
        if (playerLayers.value == 0)
            playerLayers = LayerMask.GetMask("Player", "Default");

        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
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

    // Called by BossAnimations to enable the laser
    public void ActivateLaser()
    {
        isLaserOn = true;
        lineRenderer.enabled = true;
        currentLaserLength = 0f;
        activeTimer = 0f;
    }

    // Called by BossAnimations to disable the laser
    public void DeactivateLaser()
    {
        isLaserOn = false;
        lineRenderer.enabled = false;
    }

    public void SetLaserDuration(float duration)
    {
        laserOnDuration = duration;
    }

    void UpdateLaserBeam()
    {
        float effectiveSpeed = laserExtendSpeed * 10f;
        currentLaserLength = Mathf.Min(maxLaserDistance, currentLaserLength + effectiveSpeed * Time.deltaTime);

        Vector3 startPoint = transform.position;
        Vector3 direction;

        // Use negative predicted player delta if possible
        if (delayedTarget.HasValue)
        {
            Vector3 predictedTarget = delayedTarget.Value;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                Vector3 playerPos = player.transform.position;
                Vector3 delta = Vector3.zero;
                if (rb != null)
                {
                    delta = rb.linearVelocity * predictionTime;
                }
                // Negative delta: mirror the predicted movement
                predictedTarget = playerPos - delta;
            }

            direction = (predictedTarget - startPoint).normalized;
            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;
            else
                transform.forward = direction;
        }
        else
        {
            direction = transform.forward;
        }

        Vector3 endPoint;

        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, currentLaserLength, occluderLayers))
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = startPoint + direction * currentLaserLength;
        }

        float radius = Mathf.Max(0.01f, playerHitRadius);
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

                    var end2 = SceneUtil.FindInScene<EndGameScreenforlevel2>(includeInactive: true);
                    float failTime = GameState.LevelTime;
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

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

}
