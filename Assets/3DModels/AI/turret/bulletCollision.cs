using UnityEngine;

public class bulletCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet on any collision
        Destroy(gameObject);
    }
}
