using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector]
    public int checkpointIndex; // Set by CheckpointManager automatically

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            // Only allow activation if it's the next checkpoint in order
            if (GameState.CurrentCheckpointIndex == checkpointIndex)
            {
                activated = true;
                GameState.ActivateCheckpoint();
                Debug.Log($"Checkpoint {checkpointIndex} triggered. Current: {GameState.CurrentCheckpointIndex}/{GameState.TotalCheckpoints}");
            }
            else
            {
                Debug.Log($"Checkpoint {checkpointIndex} hit out of order. Current: {GameState.CurrentCheckpointIndex}/{GameState.TotalCheckpoints}");
            }
        }
    }
}
