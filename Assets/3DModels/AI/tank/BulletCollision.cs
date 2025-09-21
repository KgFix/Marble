using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    // Called when the bullet collides with another collider
    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet GameObject
        Destroy(gameObject);
    }

   
}
