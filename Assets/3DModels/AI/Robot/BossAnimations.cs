using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossAnimations : MonoBehaviour
{
    public Animator animator;

    [Header("Attack Delay Settings")]
    [Tooltip("Minimum time (seconds) to wait between attacks.")]
    public float minAttackDelay = 3f;

    [Tooltip("Maximum time (seconds) to wait between attacks.")]
    public float maxAttackDelay = 6f;

    [Header("Laser Attack Settings")]
    [Tooltip("Laser GameObjects to control (drag your eye lasers here)")]
    public Laser3[] eyeLasers; // Assign 2 lasers in Inspector

    [Tooltip("How long the laser stays on (seconds)")]
    public float laserAttackDuration = 2f;

    [Tooltip("How much delay (seconds) between player position and laser tracking")]
    public float laserTrackDelay = 1f;

    // Attack animation state names
    private readonly string[] attackAnimations = { "Hammer_Right", "Hammer_Left", "Spin" };

    // For delayed player tracking
    private Queue<Vector3> playerPositionHistory = new Queue<Vector3>();
    private float playerTrackTimer = 0f;
    private Transform playerTransform;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Find player by tag (adjust if needed)
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        StartCoroutine(AnimationRoutine());
    }

    IEnumerator AnimationRoutine()
    {
        // Initial delay at scene start
        yield return new WaitForSeconds(4f);

        while (true)
        {
            // Randomly choose between normal attack and laser attack
            bool doLaser = (eyeLasers != null && eyeLasers.Length > 0 && Random.value < 0.5f);

            if (doLaser)
            {
                yield return StartCoroutine(LaserAttackRoutine());
            }
            else
            {
                // Pick a random attack animation
                string attack = attackAnimations[Random.Range(0, attackAnimations.Length)];
                animator.Play(attack);

                // Wait for the attack animation to finish
                yield return StartCoroutine(WaitForAnimationToEnd(attack));
            }

            // Wait a random time between minAttackDelay and maxAttackDelay seconds before next attack
            float waitTime = Random.Range(minAttackDelay, maxAttackDelay);
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator LaserAttackRoutine()
    {
        // Prepare for delayed tracking
        playerPositionHistory.Clear();
        playerTrackTimer = 0f;

        // Enable lasers
        foreach (var laser in eyeLasers)
        {
            if (laser != null)
                laser.enabled = true; // Ensure script is enabled
        }

        float elapsed = 0f;
        while (elapsed < laserAttackDuration)
        {
            elapsed += Time.deltaTime;

            // Record player position for delayed tracking
            if (playerTransform != null)
            {
                playerTrackTimer += Time.deltaTime;
                playerPositionHistory.Enqueue(playerTransform.position);

                // Remove old positions
                while (playerPositionHistory.Count > 0 && playerTrackTimer > laserTrackDelay)
                {
                    playerPositionHistory.Dequeue();
                    playerTrackTimer -= Time.deltaTime;
                }
            }

            // Set each laser to look at the delayed position
            Vector3? delayedPos = null;
            if (playerPositionHistory.Count > 0)
                delayedPos = playerPositionHistory.Peek();

            foreach (var laser in eyeLasers)
            {
                if (laser != null)
                    laser.SetTargetPosition(delayedPos);
            }

            // Activate the laser visually (if not already)
            foreach (var laser in eyeLasers)
            {
                if (laser != null && !laser.lineRenderer.enabled)
                    laser.ActivateLaser();
            }

            yield return null;
        }

        // Deactivate lasers and clear target
        foreach (var laser in eyeLasers)
        {
            if (laser != null)
            {
                laser.DeactivateLaser();
                laser.SetTargetPosition(null);
            }
        }
    }

    IEnumerator WaitForAnimationToEnd(string stateName)
    {
        // Wait for the animator to actually enter the state
        yield return null;

        int stateHash = Animator.StringToHash(stateName);

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == stateHash)
            {
                if (stateInfo.normalizedTime >= 0.98f)
                    break;
            }
            yield return null;
        }
    }
}
