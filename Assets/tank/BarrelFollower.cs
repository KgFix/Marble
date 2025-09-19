using UnityEngine;

public class BarrelFollower : MonoBehaviour
{
    public Transform player;           // Assign the player's Transform in the Inspector
    public GameObject bulletPrefab;    // Assign in Inspector
    public Transform firePoint;        // Where bullets come out
    public float bulletSpeed = 20f;
    public float fireRate = 2f;

    private float fireCooldown;

    void Update()
    {
        if (player != null)
        {
            // Rotate the barrel to look at the player (only on the Y axis)
            Vector3 lookPos = player.position - transform.position;
            lookPos.y = 0; // Keep barrel level, only rotate horizontally
            if (lookPos != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = rotation;
            }

            // Shooting cooldown
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f && CanSeePlayer())
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
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
    }
}
