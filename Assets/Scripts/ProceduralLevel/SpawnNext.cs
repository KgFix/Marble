using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GenerateNextPlatform platformManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your marble has the "Player" tag
        {
            platformManager.SpawnNextPlatform();
            // Optionally, destroy the trigger so it only fires once
            Destroy(gameObject);
        }
    }
}
