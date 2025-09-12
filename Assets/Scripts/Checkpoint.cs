using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector]
    public int checkpointIndex; // Set by CheckpointManager automatically

    [Header("Sound")]
    [SerializeField] private AudioClip checkpointSound; // Assign in Inspector
    private AudioSource audioSource;

    private bool activated = false;

    private void Awake()
    {
        // Add or get AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Optional: prevent looping
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            // Only allow activation if it's the next checkpoint in order
            if (GameState.CurrentCheckpointIndex == checkpointIndex)
            {
                activated = true;
                GameState.ActivateCheckpoint();

                // Play sound if assigned
                if (checkpointSound != null)
                    audioSource.PlayOneShot(checkpointSound);

                Debug.Log($"Checkpoint {checkpointIndex} triggered. Current: {GameState.CurrentCheckpointIndex}/{GameState.TotalCheckpoints}");
            }
            else
            {
                Debug.Log($"Checkpoint {checkpointIndex} hit out of order. Current: {GameState.CurrentCheckpointIndex}/{GameState.TotalCheckpoints}");
            }
        }
    }
}
