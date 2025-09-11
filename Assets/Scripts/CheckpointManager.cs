using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Assign checkpoints in order")]
    [SerializeField] private Checkpoint[] checkpoints;

    private void Awake()
    {
        // Assign indices automatically
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].checkpointIndex = i;
        }

        // Reset GameState
        GameState.Reset(checkpoints.Length);
    }
}
