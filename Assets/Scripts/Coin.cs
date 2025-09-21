using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;
    public float rotationSpeed = 90f;

    [Header("Collection Effects")]
    public AudioClip collectSound;
    public GameObject collectEffect; // Optional particle effect

    void Update()
    {
        // Rotate the coin for visual appeal
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Ball"))
        {
            // Add coin to the counter
            CoinManager.Instance.CollectCoin(coinValue);

            // Play collection sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Spawn particle effect
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, transform.rotation);
            }

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}
