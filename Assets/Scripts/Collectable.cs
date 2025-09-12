using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Collectable Settings")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.CollectItem();

            if (collectSound != null)
                audioSource.PlayOneShot(collectSound, volume);

            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            Destroy(gameObject, collectSound != null ? collectSound.length : 0f);
        }
    }
}
