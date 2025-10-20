using UnityEngine;

public class BarrelFollower : MonoBehaviour
{
    public Transform player;           // Assign the player's Transform in the Inspector
    public GameObject bulletPrefab;    // Assign in Inspector
    public Transform firePoint;        // Where bullets come out
    public float bulletSpeed = 20f;
    public float fireRate = 2f;
    public ParticleSystem muzzleFlash; // Assign in Inspector
    public float maxShootingDistance = 30f; // Maximum distance to shoot

    private float fireCooldown;

    void Update()
    {
        if (player != null)
        {
            Vector3 targetPos = GetPredictedPosition();

            // Rotate the barrel to look at the predicted position (only on the Y axis)
            Vector3 lookPos = targetPos - transform.position;
            lookPos.y = 0; // Keep barrel level, only rotate horizontally
            if (lookPos != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = rotation;
            }

            // Shooting cooldown
            fireCooldown -= Time.deltaTime;
            float distanceToPlayer = Vector3.Distance(firePoint.position, player.position);
            if (fireCooldown <= 0f && CanSeePlayer() && distanceToPlayer <= maxShootingDistance)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
    }

    Vector3 GetPredictedPosition()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
            return player.position;

        Vector3 playerPos = player.position;
        Vector3 playerVelocity = playerRb.linearVelocity; // Use .velocity, not .linearVelocity
        Vector3 firePos = firePoint.position;
        Vector3 toPlayer = playerPos - firePos;

        float bulletSpeedSq = bulletSpeed * bulletSpeed;
        float playerVelSq = playerVelocity.sqrMagnitude;
        float toPlayerSq = toPlayer.sqrMagnitude;

        float a = playerVelSq - bulletSpeedSq;
        float b = 2f * Vector3.Dot(toPlayer, playerVelocity);
        float c = toPlayerSq;

        float discriminant = b * b - 4f * a * c;

        float t;
        if (discriminant > 0f)
        {
            float sqrtDisc = Mathf.Sqrt(discriminant);
            float t1 = (-b + sqrtDisc) / (2f * a);
            float t2 = (-b - sqrtDisc) / (2f * a);

            // Use the smallest positive time
            t = Mathf.Min(t1, t2);
            if (t < 0f) t = Mathf.Max(t1, t2);
            if (t < 0f) t = 0f;
        }
        else
        {
            // No valid solution, fallback to direct aim
            t = 0f;
        }

        return playerPos + playerVelocity * t;
    }

    bool CanSeePlayer()
    {
        if (firePoint == null || player == null) return false;

        Vector3 origin = firePoint.position;
        Vector3 dir = (player.position - origin).normalized;
        float distance = Vector3.Distance(origin, player.position);

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, distance))
        {
            // Only shoot if the raycast hits the player
            if (hit.transform == player)
                return true;
        }
        return false;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        // Play the particle effect if assigned
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }
}
