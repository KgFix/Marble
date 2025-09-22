using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Collectable Settings")]
    public int collectableValue = 1;
    public AudioClip collectSound;
    public float volume = 1f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.CompareTag("Player") || other.name.Contains("Ball"))
        {
            collected = true;

            if (CollectableManager.Instance != null)
                CollectableManager.Instance.Collect(this);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);

            foreach (var renderer in GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
            foreach (var collider in GetComponents<Collider>())
                collider.enabled = false;

            Destroy(gameObject, 0.2f);
        }
    }
}
