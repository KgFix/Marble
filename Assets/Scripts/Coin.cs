using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;
    public float rotationSpeed = 90f;

    [Header("Sound")]
    public AudioClip collectSound;
    public float volume = 1f;

    void Update()
    {
        // Simple rotation for visual flair
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Ball"))
        {
            CoinManager.Instance.CollectCoin(coinValue);

            // Play only the sound
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }
    }
}
